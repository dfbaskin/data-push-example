namespace RecordProxy.Generator;

public class RecordArrayProxy<TModel, TModelProxy>
    where TModel : class
    where TModelProxy : class, TModel
{
    private readonly ChangeCollectorState state;
    private readonly IProxyWrapper<TModel, TModelProxy> wrapper;
    private List<TModel> items;

    public RecordArrayProxy(
        ChangeCollectorState state,
        IProxyWrapper<TModel, TModelProxy> wrapper,
        IEnumerable<TModel>? items = null
    )
    {
        this.state = state ?? throw new ArgumentNullException(nameof(state));
        this.wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
        this.items = new List<TModel>(items ?? Enumerable.Empty<TModel>());
    }

    public int Count => items.Count;

    public RecordArrayProxy<TModel, TModelProxy> Add(TModel item)
    {
        var index = items.Count;
        state.Handler.Add(state.ModelPath.AddPath(index), item);

        var proxy = wrapper.Wrap(item, state.AddPath(index));
        items.Add(proxy);

        return this;
    }

    public ICollection<TModel> ToCollection(RecordArrayProxy<TModel, TModelProxy>? updated = null)
    {
        return (updated ?? this).Unwrap().ToList();
    }

    public IEnumerable<TModel> Unwrap()
    {
        return items
            .Select(item =>
                item is TModelProxy proxy
                    ? wrapper.UnWrap(proxy)
                    : item
            );
    }
}
