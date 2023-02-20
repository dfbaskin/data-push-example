using RecordProxy.Generator;

internal sealed class TransportInstanceUpdater : ModelInstanceUpdater<Transport, TransportProxy>
{
    public ModelInstanceUpdaterContext ModelContext { get; }

    public TransportInstanceUpdater(ModelInstanceUpdaterContext modelContext)
    {
        ModelContext = modelContext ?? throw new ArgumentNullException(nameof(modelContext));
    }

    protected override string GetInstanceId(TransportInstanceContext context)
        => context.TransportId;

    protected override TransportInstanceContext UpdateContext(TransportInstanceContext context, Transport updated)
        => context with {
            Transport = updated
        };

    protected override UpdatedItem<Transport>? UpdateItem(IChangeCollector collector, string instanceId)
    {
        return ModelContext.Current.UpdateTransport(instanceId, original =>
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
