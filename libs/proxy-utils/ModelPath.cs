namespace RecordProxy.Generator;

public abstract record ModelPath
{
    public ModelPath()
    {
        Parent = null;
    }
    public ModelPath(ModelPath? parent)
    {
        Parent = parent;
    }

    public ModelPath? Parent { get; init; }

    public bool IsRoot => Parent == null;

    public IEnumerable<ModelPath> AllPathSegments() => GetAllPathSegments().Reverse();
    private IEnumerable<ModelPath> GetAllPathSegments()
    {
        ModelPath? current = this;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }
}

public record ModelPathProperty(
    string PropertyName,
    ModelPath? Parent = null
) : ModelPath(Parent);

public record ModelPathArrayIndex(
    int ArrayIndex,
    ModelPath? Parent = null
) : ModelPath(Parent);

public static class ModelPathExtensions
{
    public static ModelPathProperty AddPath(this ModelPath? modelPath, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace.", nameof(propertyName));
        }

        return new ModelPathProperty(
            PropertyName: propertyName,
            Parent: modelPath
        );
    }

    public static ModelPathArrayIndex AddPath(this ModelPath? modelPath, int arrayIndex)
    {
        if (arrayIndex < 0)
        {
            throw new ArgumentException($"'{nameof(arrayIndex)}' must not be less than zero.", nameof(arrayIndex));
        }

        return new ModelPathArrayIndex(
            ArrayIndex: arrayIndex,
            Parent: modelPath
        );
    }
}
