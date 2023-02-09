public sealed partial class SimulationWorker
{
    private ItemUpdater<Driver> UpdateDriver()
    {
        var context = new ItemUpdaterContext<Driver>(
            UpdateItem: Current.UpdateDriver,
            UpdateTransportContext: (entity, context) => {
                return context with {
                    Driver = entity
                };
            },
            ModifyHistory: (history, entity) => {
                return entity with {
                    History = history
                };
            },
            GetItemId: context => context.DriverId,
            SendNotifications: SendDriverUpdates
        );

        return new ItemUpdater<Driver>(context);
    }

    private async Task SendDriverUpdates(UpdatedItem<Driver> result)
    {
        var original = result.Original;
        var updated = result.Updated;

        await SendDriverUpdate(updated);

        async Task CheckForGroupChange(string? groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                var group = Current.Groups.Where(g => g.Name == groupName).Single();
                await SendGroupUpdate(group);
            }
        }

        if (original.GroupAssignment != updated.GroupAssignment)
        {
            await CheckForGroupChange(original.GroupAssignment);
            await CheckForGroupChange(updated.GroupAssignment);
        }

        var transport = Current.Transports
            .Where(t => t.Status != TransportStatus.Finished && t.DriverId == updated.DriverId)
            .FirstOrDefault();
        if (transport != null)
        {
            await SendTransportUpdate(transport);
        }
    }

    private async Task SendDriverUpdate(Driver updated)
    {
        await Sender.SendAsync(nameof(Subscription.DriverUpdated), updated);
        await Sender.SendAsync($"DriverByIdUpdated_{updated.DriverId}", updated);
    }
}
