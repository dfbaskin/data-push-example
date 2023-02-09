public sealed partial class SimulationWorker
{
    private async Task SendGroupUpdates(Group updated)
    {
        await Sender.SendAsync(nameof(Subscription.GroupUpdated), updated);

        await Sender.SendAsync(
            $"GroupByNameUpdated_{updated.Name}",
            updated);
    }
}
