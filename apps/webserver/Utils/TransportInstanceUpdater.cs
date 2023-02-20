using RecordProxy.Generator;

internal sealed class TransportInstanceUpdater : ModelInstanceUpdater<Transport, TransportProxy>
{
    public CurrentData Current { get; }

    public TransportInstanceUpdater(CurrentData current)
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
    }

    protected override string GetInstanceId(TransportInstanceContext context)
        => context.TransportId;

    protected override TransportInstanceContext UpdateContext(TransportInstanceContext context, Transport updated)
        => context with {
            Transport = updated
        };

    protected override UpdatedItem<Transport>? UpdateItem(IChangeCollector collector, string instanceId)
    {
        return Current.UpdateTransport(instanceId, original =>
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
