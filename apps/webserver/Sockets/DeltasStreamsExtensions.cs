public static class DeltasStreamsExtensions
{
    public static IApplicationBuilder UseDeltasWebSocketStreams(this IApplicationBuilder app)
    {
        return app.UseMiddleware<DeltasStreamsMiddleware>();
    }
}
