namespace RecordProxy.Generator;

public interface IChangeCollector
{
    void Add(ModelPath modelPath, object? value);
    void Replace(ModelPath modelPath, object? value);
    T ReplaceIfChanged<T>(ModelPath modelPath, T original, T updated)
        where T : IEquatable<T>;
}
