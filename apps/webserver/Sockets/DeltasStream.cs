
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using System.Threading.Channels;

public class DeltasStream
{
    private static long nextConnectionId = 0;
    private static readonly ConcurrentDictionary<long, DeltasState> queues
        = new ConcurrentDictionary<long, DeltasState>();

    private int ConnectionCount => queues.Count;

    public static async Task OnDataUpdated(DeltasStreamUpdated update)
    {
        foreach (var queue in queues.Values)
        {
            await queue.Channel.Writer.WriteAsync(update);
        }
    }

    public DeltasStream(
        CurrentData current,
        IHostApplicationLifetime lifetime,
        ILogger<DeltasStream> logger
    )
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
        Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public CurrentData Current { get; }
    public IHostApplicationLifetime Lifetime { get; }
    public ILogger<DeltasStream> Logger { get; }

    public async Task StreamData(WebSocket socket)
    {
        void LogActiveConnections()
            => Logger.LogInformation("{connectionCount} active connections.", ConnectionCount);
        var connectionId = Interlocked.Increment(ref nextConnectionId);
        try
        {
            Logger.LogInformation("Started streaming updates (#{connectionId})", connectionId);

            var state = new DeltasState(
                ConnectionId: connectionId,
                Channel: Channel.CreateUnbounded<DeltasStreamUpdated>(),
                Socket: socket
            );
            queues.TryAdd(connectionId, state);
            LogActiveConnections();

            var sendTask = SendDeltaChanges(state, Lifetime.ApplicationStopping)
                .ContinueWith(t =>
                {
                    Logger.LogInformation("Send task shut down (#{connectionId})", connectionId);
                });
            var receiveTask = ProcessStreamRequests(state, Lifetime.ApplicationStopping)
                .ContinueWith(t =>
                {
                    Logger.LogInformation("Receive task shut down (#{connectionId})", connectionId);
                    state.Channel.Writer.Complete();
                });

            await Task.WhenAll(sendTask, receiveTask);

            Logger.LogInformation("Finished streaming updates (#{connectionId})", connectionId);
        }
        finally
        {
            queues.Remove(connectionId, out var removedItem);
            LogActiveConnections();
        }
    }

    private async Task SendDeltaChanges(DeltasState state, CancellationToken token)
    {
        while (await state.Channel.Reader.WaitToReadAsync(token))
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            await foreach (var updatedStream in state.Channel.Reader.ReadAllAsync(token))
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (!state.RequestedStreams.Any(r => r.Matches(updatedStream)))
                {
                    continue;
                }

                var buffer = System.Text.Encoding.UTF8.GetBytes(updatedStream.JsonDocument);
                await state.Socket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text,
                    true,
                    token
                );
            }
        }
    }

    private async Task ProcessStreamRequests(DeltasState state, CancellationToken token)
    {
        var buffer = new byte[1024];
        while (!state.Socket.CloseStatus.HasValue && !token.IsCancellationRequested)
        {
            var result = await state.Socket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                token
            );

            if (token.IsCancellationRequested)
            {
                break;
            }

            if (result.MessageType == WebSocketMessageType.Text && result.EndOfMessage)
            {
                var requestBytes = new ArraySegment<byte>(buffer, 0, result.Count);
                try
                {
                    var request = JsonSerializer.Deserialize<DeltasStreamRequest>(
                        requestBytes,
                        JsonUtils.SerializerOptions
                    );
                    if (request != null)
                    {
                        state.UpdatedRequestedStreams(request with
                        {
                            Subscribe = false
                        });
                        if (request.Subscribe)
                        {
                            await QueueInitialDocument(state, request);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing incoming websocket message.");
                    Logger.LogInformation("Message: {message}", Encoding.UTF8.GetString(requestBytes));
                }
            }
        }

        await state.Socket.CloseAsync(
            WebSocketCloseStatus.NormalClosure,
            null,
            CancellationToken.None
        );
    }

    private async Task QueueInitialDocument(DeltasState state, DeltasStreamRequest request)
    {

        switch (request.StreamType)
        {
            case DeltasStreamType.Groups:
                break;
            case DeltasStreamType.Geolocation:
                break;
            case DeltasStreamType.Transports:
                {
                    state.UpdatedRequestedStreams(request);
                    await state.Channel.Writer.WriteAsync(
                        TransportGridView.InitialData(
                            Current.Transports,
                            Current.Drivers,
                            Current.Vehicles
                        )
                    );
                }
                break;
            case DeltasStreamType.TransportDetails:
                {
                    var transport = Current.Transports.FirstOrDefault(t => t.TransportId == request.Id);
                    var driver = Current.Drivers.FirstOrDefault(d => d.DriverId == transport?.DriverId);
                    var vehicle = Current.Vehicles.FirstOrDefault(v => v.VehicleId == transport?.VehicleId);
                    if (transport != null && driver != null && vehicle != null)
                    {
                        state.UpdatedRequestedStreams(request);
                        await state.Channel.Writer.WriteAsync(
                            TransportDetailsView.InitialData(transport, driver, vehicle)
                        );
                    }
                }
                break;
            case DeltasStreamType.VehicleDetails:
                {
                    var vehicle = Current.Vehicles.FirstOrDefault(d => d.VehicleId == request.Id);
                    if (vehicle != null)
                    {
                        state.UpdatedRequestedStreams(request);
                        await state.Channel.Writer.WriteAsync(
                            VehicleDetailsView.InitialData(vehicle)
                        );
                    }
                }
                break;
            case DeltasStreamType.DriverDetails:
                {
                    var driver = Current.Drivers.FirstOrDefault(d => d.DriverId == request.Id);
                    if (driver != null)
                    {
                        state.UpdatedRequestedStreams(request);
                        await state.Channel.Writer.WriteAsync(
                            DriverDetailsView.InitialData(driver)
                        );
                    }
                }
                break;
        }
    }

    private record DeltasState(
        long ConnectionId,
        Channel<DeltasStreamUpdated> Channel,
        WebSocket Socket
    )
    {
        public IReadOnlyCollection<DeltasStreamRequest> RequestedStreams { get; private set; }
            = new List<DeltasStreamRequest>();

        public void UpdatedRequestedStreams(DeltasStreamRequest request)
        {
            RequestedStreams = GetUpdatedRequestedStreamList(request).ToList();
        }

        private IEnumerable<DeltasStreamRequest> GetUpdatedRequestedStreamList(DeltasStreamRequest request)
        {
            bool found = false;
            foreach (var currentStream in RequestedStreams)
            {
                if (request.ShouldReplace(currentStream))
                {
                    found = true;
                    if (request.Subscribe)
                    {
                        yield return request;
                    }
                }
                else
                {
                    yield return currentStream;
                }
            }

            if (!found)
            {
                if (request.Subscribe)
                {
                    yield return request;
                }
            }
        }
    }
}
