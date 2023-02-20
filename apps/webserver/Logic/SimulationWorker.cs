using HotChocolate.Subscriptions;

public sealed partial class SimulationWorker : BackgroundService
{
    public SimulationWorker(
        ModelInstanceUpdaterContext modelContext,
        ILogger<SimulationWorker> logger
    )
    {
        ModelContext = modelContext ?? throw new ArgumentNullException(nameof(modelContext));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ModelInstanceUpdaterContext ModelContext { get; }
    public CurrentData Current => ModelContext.Current;
    public ITopicEventSender Sender => ModelContext.Sender;
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
                {
                    try
                    {
                        Interlocked.Increment(ref currentTaskCount);
                        await TransportItems(context);
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

    private async Task WaitForAFewSeconds(SimulationContext context)
    {
        int seconds = Faker.RandomNumber.Next(3, 7);
        await Task.Delay(seconds * 1000, context.Token);
    }
}
