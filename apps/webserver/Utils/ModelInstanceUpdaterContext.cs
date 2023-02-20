using HotChocolate.Subscriptions;

public class ModelInstanceUpdaterContext
{
    public ModelInstanceUpdaterContext(
        CurrentData current,
        ITopicEventSender sender
    )
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
        Sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public CurrentData Current { get; }
    public ITopicEventSender Sender { get; }
}
