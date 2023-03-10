using HotChocolate.Subscriptions;

public sealed partial class SimulationWorker : BackgroundService
{
    public SimulationWorker(
        CurrentData current,
        ITopicEventSender sender,
        ILogger<SimulationWorker> logger
    )
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
        Sender = sender ?? throw new ArgumentNullException(nameof(sender));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public CurrentData Current { get; }
    public ITopicEventSender Sender { get; }
    public ILogger<SimulationWorker> Logger { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var context = new Context(
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

    private async Task WaitForAFewSeconds(Context context)
    {
        int seconds = Faker.RandomNumber.Next(3, 7);
        await Task.Delay(seconds * 1000, context.Token);
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
