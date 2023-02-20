using RecordProxy.Generator;

internal sealed class VehicleInstanceUpdater : ModelInstanceUpdater<Vehicle, VehicleProxy>
{
    public ModelInstanceUpdaterContext ModelContext { get; }

    public VehicleInstanceUpdater(ModelInstanceUpdaterContext modelContext)
    {
        ModelContext = modelContext ?? throw new ArgumentNullException(nameof(modelContext));
    }

    protected override string GetInstanceId(TransportInstanceContext context)
        => context.VehicleId;

    protected override TransportInstanceContext UpdateContext(TransportInstanceContext context, Vehicle updated)
        => context with
        {
            Vehicle = updated
        };

    protected override UpdatedItem<Vehicle>? UpdateItem(IChangeCollector collector, string instanceId)
    {
        return ModelContext.Current.UpdateVehicle(instanceId, original =>
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

    protected override async Task SendNotifications(UpdatedItem<Vehicle> result)
    {
        var updated = result.Updated;

        await ModelContext.Subscriptions.SendVehicleUpdate(updated);

        var transport = ModelContext.Current.Transports
            .Where(t => t.Status != TransportStatus.Finished && t.VehicleId == updated.VehicleId)
            .FirstOrDefault();
        if (transport != null)
        {
            await ModelContext.Subscriptions.SendTransportUpdate(transport);
        }
    }
}
