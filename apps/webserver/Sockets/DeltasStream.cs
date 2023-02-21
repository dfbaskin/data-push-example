
using System.Net.WebSockets;

public class DeltasStream
{
    private static int connectionCount = 0;

    public DeltasStream(ILogger<DeltasStream> logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ILogger<DeltasStream> Logger { get; }

    public async Task StreamData(WebSocket socket)
    {
        void LogActiveConnections(int count)
            => Logger.LogInformation("{connectionCount} active connections.", count);

        try
        {
            Logger.LogInformation("Started streaming updates");
            LogActiveConnections(Interlocked.Increment(ref connectionCount));

            var buffer = new byte[1024 * 4];
            var receiveResult = await socket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );

            while (!receiveResult.CloseStatus.HasValue)
            {
                await socket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None
                );

                receiveResult = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );
            }

            await socket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None
            );

            Logger.LogInformation("Finished streaming updates.");
        }
        finally
        {
            LogActiveConnections(Interlocked.Decrement(ref connectionCount));
        }
    }
}
