public class ModelInstanceUpdaterContext
{
    public ModelInstanceUpdaterContext(
        CurrentData current,
        SubscriptionUpdates subscriptions
    )
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
        Subscriptions = subscriptions ?? throw new ArgumentNullException(nameof(subscriptions));
    }

    public CurrentData Current { get; }
    public SubscriptionUpdates Subscriptions { get; }
}
