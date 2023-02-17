namespace RecordProxy.Generator;

public interface IProxyWrapper<TModel, TModelProxy>
    where TModel : class
    where TModelProxy : class
{
    TModelProxy Wrap(TModel model, ChangeCollectorState collector);
    TModel UnWrap(TModelProxy model);
}
