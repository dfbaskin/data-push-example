using RecordProxy.Generator;

internal sealed class DriverInstanceUpdater : ModelInstanceUpdater<Driver, DriverProxy>
{
    public CurrentData Current { get; }

    public DriverInstanceUpdater(CurrentData current)
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
    }

    protected override string GetInstanceId(TransportInstanceContext context)
        => context.DriverId;

    protected override TransportInstanceContext UpdateContext(TransportInstanceContext context, Driver updated)
        => context with {
            Driver = updated
        };

    protected override UpdatedItem<Driver>? UpdateItem(IChangeCollector collector, string instanceId)
    {
        return Current.UpdateDriver(instanceId, original =>
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
