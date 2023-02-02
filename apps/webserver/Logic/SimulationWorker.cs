public sealed partial class SimulationWorker : BackgroundService
{
    public SimulationWorker(
        CurrentData current,
        ILogger<SimulationWorker> logger
    )
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public CurrentData Current { get; }
    public ILogger<SimulationWorker> Logger { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var context = new Context(
            Token: stoppingToken
        );

        // while (!stoppingToken.IsCancellationRequested)
        await TransportItems(context);
    }

    private record Context(
        CancellationToken Token
    )
    {
        public void VerifyIsActive()
        {
            Token.ThrowIfCancellationRequested();
        }
    }
}
