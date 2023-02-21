using Microsoft.AspNetCore.JsonPatch;
using RecordProxy.Generator;

internal abstract class ModelInstanceUpdater<TModel, TModelProxy>
    where TModel : class
    where TModelProxy : class
{
    protected Func<TModelProxy, TModelProxy>? ModifyFn { get; private set; }
    protected List<HistoryEntry>? HistoryToAdd { get; private set; }
    private string? InstanceId { get; set; }

    public ModelInstanceUpdater()
    {
    }

    public ModelInstanceUpdater<TModel, TModelProxy> WithId(string instanceId)
    {
        InstanceId = instanceId;
        return this;
    }

    public ModelInstanceUpdater<TModel, TModelProxy> Modify(Func<TModelProxy, TModelProxy> modifyFn)
    {
        if (ModifyFn != null)
        {
            throw new InvalidOperationException("Modify function has already been previously set.");
        }

        ModifyFn = modifyFn ?? throw new ArgumentNullException(nameof(modifyFn));
        return this;
    }

    public ModelInstanceUpdater<TModel, TModelProxy> AddHistory(string message)
    {
        var history = this.HistoryToAdd ?? (this.HistoryToAdd = new List<HistoryEntry>());
        history.Add(HistoryEntry.CreateHistoryEntry(message));
        return this;
    }

    public async Task<TModel?> Update()
    {
        var result = await UpdateItem();
        if (result == null)
        {
            return null;
        }

        return result.Updated;
    }

    public async Task<TransportInstanceContext> Update(TransportInstanceContext context)
    {
        InstanceId = string.IsNullOrWhiteSpace(InstanceId) ? GetInstanceId(context) : InstanceId;

        var result = await UpdateItem();
        if (result == null)
        {
            throw new InvalidOperationException($"Could not find {typeof(TModel).Name} entity in collection.");
        }

        return UpdateContext(context, result.Updated);
    }

    protected abstract string GetInstanceId(TransportInstanceContext context);
    protected abstract TransportInstanceContext UpdateContext(TransportInstanceContext context, TModel updated);
    protected abstract Task SendNotifications(UpdatedItem<TModel> result, JsonPatchDocument patches);
    protected abstract UpdatedItem<TModel>? UpdateItem(IChangeCollector collector, string instanceId);

    private async Task<UpdatedItem<TModel>?> UpdateItem()
    {
        if (string.IsNullOrWhiteSpace(InstanceId))
        {
            throw new InvalidOperationException($"Id of {typeof(TModel).Name} entity must be provided.");
        }

        var collector = new JsonPatchCollector();

        var result = UpdateItem(collector, InstanceId);

        if (result != null)
        {
            await SendNotifications(result, collector.GetOperations());
        }

        return result;
    }
}
