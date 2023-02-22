
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Channels;

public class DeltasStream
{
    private record DeltasState(
        long ConnectionId,
        Channel<DeltasStreamUpdated> Channel,
        WebSocket Socket
    )
    {
        public IReadOnlyCollection<DeltasStreamRequest> RequestedStreams { get; set; }
            = new List<DeltasStreamRequest>();
    }

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
        ILogger<DeltasStream> logger
    )
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public CurrentData Current { get; }
    public ILogger<DeltasStream> Logger { get; }

    public async Task StreamData(WebSocket socket)
    {
        void LogActiveConnections()
            => Logger.LogInformation("{connectionCount} active connections.", ConnectionCount);
        var connectionId = Interlocked.Increment(ref nextConnectionId);
        try
        {
            Logger.LogInformation("Started streaming updates ({connectionId})", connectionId);
            LogActiveConnections();

            var state = new DeltasState(
                ConnectionId: connectionId,
                Channel: Channel.CreateUnbounded<DeltasStreamUpdated>(),
                Socket: socket
            );
            queues.TryAdd(connectionId, state);

            var tokenSource = new CancellationTokenSource();

            var sendTask = SendDeltaChanges(state, tokenSource.Token);
            var receiveTask = ProcessStreamRequests(state, tokenSource.Token)
                .ContinueWith(t =>
                {
                    tokenSource.Cancel();
                });

            await Task.WhenAll(sendTask, receiveTask);

            Logger.LogInformation("Finished streaming updates.");
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

            if (result.MessageType == WebSocketMessageType.Text && result.EndOfMessage)
            {
                try
                {
                    var requestBytes = new ArraySegment<byte>(buffer, 0, result.Count);
                    var request = JsonSerializer.Deserialize<DeltasStreamRequest>(
                        requestBytes,
                        JsonUtils.SerializerOptions
                    );
                    if (request != null)
                    {
                        state.RequestedStreams = GetUpdatedRequestedStreamList(state, request).ToList();
                        if (request.Subscribe)
                        {
                            await QueueInitialDocument(state, request);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing incoming websocket message.");
                }
            }
        }

        await state.Socket.CloseAsync(
            WebSocketCloseStatus.NormalClosure,
            null,
            token
        );

        state.Channel.Writer.Complete();
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
                break;
            case DeltasStreamType.TransportDetails:
                var transport = Current.Transports.FirstOrDefault(t => t.TransportId == request.Id);
                if (transport != null)
                {
                    await state.Channel.Writer.WriteAsync(
                        DeltasStreamUpdated.ForInitialDocument(
                            DeltasStreamType.TransportDetails,
                            transport.TransportId,
                            transport
                        )
                    );
                }
                break;
            case DeltasStreamType.VehicleDetails:
                var vehicle = Current.Vehicles.FirstOrDefault(t => t.VehicleId == request.Id);
                if (vehicle != null)
                {
                    await state.Channel.Writer.WriteAsync(
                        DeltasStreamUpdated.ForInitialDocument(
                            DeltasStreamType.VehicleDetails,
                            vehicle.VehicleId,
                            vehicle
                        )
                    );
                }
                break;
            case DeltasStreamType.DriverDetails:
                var driver = Current.Drivers.FirstOrDefault(t => t.DriverId == request.Id);
                if (driver != null)
                {
                    await state.Channel.Writer.WriteAsync(
                        DeltasStreamUpdated.ForInitialDocument(
                            DeltasStreamType.DriverDetails,
                            driver.DriverId,
                            driver
                        )
                    );
                }
                break;
        }
    }

    private IEnumerable<DeltasStreamRequest> GetUpdatedRequestedStreamList(DeltasState state, DeltasStreamRequest request)
    {
        bool found = false;
        foreach (var requestedStream in state.RequestedStreams)
        {
            if (request.ShouldReplace(requestedStream))
            {
                found = true;
                if (request.Subscribe)
                {
                    yield return request;
                }
            }
            else
            {
                yield return requestedStream;
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
