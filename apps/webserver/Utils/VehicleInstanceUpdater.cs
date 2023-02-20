using RecordProxy.Generator;

internal sealed class VehicleInstanceUpdater : ModelInstanceUpdater<Vehicle, VehicleProxy>
{
    public CurrentData Current { get; }

    public VehicleInstanceUpdater(CurrentData current)
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
    }

    protected override string GetInstanceId(TransportInstanceContext context)
        => context.VehicleId;

    protected override TransportInstanceContext UpdateContext(TransportInstanceContext context, Vehicle updated)
        => context with {
            Vehicle = updated
        };

    protected override UpdatedItem<Vehicle>? UpdateItem(IChangeCollector collector, string instanceId)
    {
        return Current.UpdateVehicle(instanceId, original =>
            original.Capture(collector, proxy =>
            {
                if (ModifyFn != null)
                {
                    proxy = ModifyFn(proxy);
                }

                if (HistoryToAdd != null)
                {
                    proxy = proxy with {
                        History = proxy.History.Add(HistoryToAdd)
                    };
                }

                return proxy;
            })
        );
    }
}
