using RecordProxy.Generator;

internal sealed class DriverInstanceUpdater : ModelInstanceUpdater<Driver, DriverProxy>
{
    public ModelInstanceUpdaterContext ModelContext { get; }

    public DriverInstanceUpdater(ModelInstanceUpdaterContext modelContext)
    {
        ModelContext = modelContext ?? throw new ArgumentNullException(nameof(modelContext));
    }

    protected override string GetInstanceId(TransportInstanceContext context)
        => context.DriverId;

    protected override TransportInstanceContext UpdateContext(TransportInstanceContext context, Driver updated)
        => context with
        {
            Driver = updated
        };

    protected override UpdatedItem<Driver>? UpdateItem(IChangeCollector collector, string instanceId)
    {
        return ModelContext.Current.UpdateDriver(instanceId, original =>
            original.Capture(collector, proxy =>
            {
                if (ModifyFn != null)
                {
                    proxy = ModifyFn(proxy);
                }

                if (HistoryToAdd != null)
                {
                    proxy = proxy with
                    {
                        History = proxy.History.Add(HistoryToAdd)
                    };
                }

                return proxy;
            })
        );
    }

    protected override async Task SendNotifications(UpdatedItem<Driver> result)
    {
        var original = result.Original;
        var updated = result.Updated;

        await ModelContext.Subscriptions.SendDriverUpdate(updated);

        async Task CheckForGroupChange(string? groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                var group = ModelContext.Current.Groups.Where(g => g.Name == groupName).Single();
                await ModelContext.Subscriptions.SendGroupUpdate(group);
            }
        }

        if (original.GroupAssignment != updated.GroupAssignment)
        {
            await CheckForGroupChange(original.GroupAssignment);
            await CheckForGroupChange(updated.GroupAssignment);
        }

        var transport = ModelContext.Current.Transports
            .Where(t => t.Status != TransportStatus.Finished && t.DriverId == updated.DriverId)
            .FirstOrDefault();
        if (transport != null)
        {
            await ModelContext.Subscriptions.SendTransportUpdate(transport);
        }
    }
}
