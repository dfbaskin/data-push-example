public sealed partial class SimulationWorker
{
    private ItemUpdater<Vehicle> UpdateVehicle()
    {
        var context = new ItemUpdaterContext<Vehicle>(
            UpdateItem: Current.UpdateVehicle,
            UpdateTransportContext: (entity, context) => {
                return context with {
                    Vehicle = entity
                };
            },
            ModifyHistory: (history, entity) => {
                return entity with {
                    History = history
                };
            },
            GetItemId: context => context.VehicleId,
            SendNotifications: SendVehicleUpdates
        );

        return new ItemUpdater<Vehicle>(context);
    }

    private async Task SendVehicleUpdates(UpdatedItem<Vehicle> result)
    {
        var updated = result.Updated;

        await Sender.SendAsync(nameof(Subscription.VehicleUpdated), updated);

        await Sender.SendAsync(
            $"VehicleByIdUpdated_{updated.VehicleId}",
            updated);

        var transport = Current.Transports
            .Where(t => t.VehicleId == updated.VehicleId && t.Status != TransportStatus.Finished)
            .FirstOrDefault();
        if (transport != null)
        {
            await Sender.SendAsync(nameof(Subscription.TransportUpdated), transport);
        }
    }
}
