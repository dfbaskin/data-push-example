public class DeltasStreamsMiddleware
{
    private readonly RequestDelegate _next;

    public DeltasStreamsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DeltasStream stream)
    {
        if (context.Request.Path == "/deltas-stream")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await stream.StreamData(webSocket);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await _next(context);
        }
    }
}
