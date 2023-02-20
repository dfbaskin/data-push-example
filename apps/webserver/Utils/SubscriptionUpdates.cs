using HotChocolate.Subscriptions;

public class SubscriptionUpdates
{
    public SubscriptionUpdates(
        ITopicEventSender sender
    )
    {
        Sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public ITopicEventSender Sender { get; }

    public async Task SendGroupUpdate(Group updated)
    {
        await Sender.SendAsync(nameof(Subscription.GroupUpdated), updated);
        await Sender.SendAsync($"GroupByNameUpdated_{updated.Name}", updated);
    }

    public async Task SendDriverUpdate(Driver updated)
    {
        await Sender.SendAsync(nameof(Subscription.DriverUpdated), updated);
        await Sender.SendAsync($"DriverByIdUpdated_{updated.DriverId}", updated);
    }

    public async Task SendTransportUpdate(Transport updated)
    {
        await Sender.SendAsync(nameof(Subscription.TransportUpdated), updated);
        await Sender.SendAsync($"TransportByIdUpdated_{updated.TransportId}", updated);
    }

    public async Task SendVehicleUpdate(Vehicle updated)
    {
        await Sender.SendAsync(nameof(Subscription.VehicleUpdated), updated);
        await Sender.SendAsync($"VehicleByIdUpdated_{updated.VehicleId}", updated);
    }
}
