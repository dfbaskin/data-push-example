public sealed partial class SimulationWorker
{
    private ItemUpdater<Transport> UpdateTransport()
    {
        var context = new ItemUpdaterContext<Transport>(
            UpdateItem: Current.UpdateTransport,
            UpdateTransportContext: (entity, context) => {
                return context with {
                    Transport = entity
                };
            },
            ModifyHistory: (history, entity) => {
                return entity with {
                    History = history
                };
            },
            GetItemId: context => context.TransportId,
            SendNotifications: SendTransportUpdates
        );

        return new ItemUpdater<Transport>(context);
    }

    private async Task SendTransportUpdates(UpdatedItem<Transport> result)
    {
        var updated = result.Updated;

        await Sender.SendAsync(nameof(Subscription.TransportUpdated), updated);
    }
}
