namespace RecordProxy.Generator;

public record ChangeCollectorState(
    IChangeCollector Handler,
    ModelPath? ModelPath = null
)
{
    public ChangeCollectorState AddPath(string propName)
    {
        return new ChangeCollectorState(
            Handler: Handler,
            ModelPath: ModelPath.AddPath(propName)
        );
    }

    public ChangeCollectorState AddPath(int arrayIndex)
    {
        return new ChangeCollectorState(
            Handler: Handler,
            ModelPath: ModelPath.AddPath(arrayIndex)
        );
    }
}
