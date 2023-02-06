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
        await Task.CompletedTask;
    }
}
