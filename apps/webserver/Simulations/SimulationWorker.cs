public sealed partial class SimulationWorker : BackgroundService
{
    public SimulationWorker(
        TransportsSimulation transports,
        ILogger<SimulationWorker> logger
    )
    {
        Transports = transports ?? throw new ArgumentNullException(nameof(transports));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public TransportsSimulation Transports { get; }
    public ILogger<SimulationWorker> Logger { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var context = new SimulationContext(
            Token: stoppingToken
        );

        int currentTaskCount = 0;
        const int maxTaskCount = 30;
        while (!stoppingToken.IsCancellationRequested)
        {
            if (currentTaskCount < maxTaskCount)
            {
#pragma warning disable 4014
                Task.Run(async () =>
#pragma warning restore 4014
                {
                    try
                    {
                        Interlocked.Increment(ref currentTaskCount);
                        await Transports.RunSimulation(context);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error executing task.");
                    }
                    finally
                    {
                        Interlocked.Decrement(ref currentTaskCount);
                    }
                }, stoppingToken);
            }

            int seconds = Faker.RandomNumber.Next(2, 10);
            await Task.Delay(seconds * 1000, context.Token);
        }
    }
}
