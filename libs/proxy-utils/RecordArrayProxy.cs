namespace RecordProxy.Generator;

public record RecordArrayProxy<TModel, TModelProxy>(
    ChangeCollectorState State,
    IProxyWrapper<TModel, TModelProxy> Wrapper,
    ICollection<TModel>? Items = null
)
    where TModel : class
    where TModelProxy : class, TModel
{
    public int Count => Items?.Count ?? 0;

    public RecordArrayProxy<TModel, TModelProxy> Add(TModel item)
    {
        var index = Count;
        State.Handler.Add(State.ModelPath.AddPath(index), item);

        var proxy = Wrapper.Wrap(item, State.AddPath(index));
        var list = GetItems().Concat(Enumerable.Repeat(item, 1)).ToList();

        return new RecordArrayProxy<TModel, TModelProxy>(
            State: State,
            Wrapper: Wrapper,
            Items: list
        );
    }

    public RecordArrayProxy<TModel, TModelProxy> Add(IEnumerable<TModel> items)
    {
        var list = GetItems().ToList();
        foreach (var item in items)
        {
            var index = Count;
            State.Handler.Add(State.ModelPath.AddPath(index), item);

            var proxy = Wrapper.Wrap(item, State.AddPath(index));
            list.Add(item);
        }

        return new RecordArrayProxy<TModel, TModelProxy>(
            State: State,
            Wrapper: Wrapper,
            Items: list
        );
    }

    public ICollection<TModel> ToCollection(RecordArrayProxy<TModel, TModelProxy>? updated = null)
    {
        return (updated ?? this).Unwrap().ToList();
    }

    public IEnumerable<TModel> Unwrap()
    {
        return GetItems()
            .Select(item =>
                item is TModelProxy proxy
                    ? Wrapper.UnWrap(proxy)
                    : item
            );
    }

    private IEnumerable<TModel> GetItems()
    {
        return Items ?? Enumerable.Empty<TModel>();
    }
}
