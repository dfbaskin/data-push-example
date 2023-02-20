public sealed partial class SimulationWorker
{
    private TransportInstanceUpdater UpdateTransport()
        => new TransportInstanceUpdater(ModelContext);

    private async Task SendTransportUpdates(UpdatedItem<Transport> result)
    {
        var updated = result.Updated;
        await SendTransportUpdate(updated);
    }

    private async Task SendTransportUpdate(Transport updated)
    {
        await Sender.SendAsync(nameof(Subscription.TransportUpdated), updated);
        await Sender.SendAsync($"TransportByIdUpdated_{updated.TransportId}", updated);
    }
}
