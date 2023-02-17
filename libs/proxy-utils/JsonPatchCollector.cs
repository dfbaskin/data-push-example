using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;

namespace RecordProxy.Generator;

public class JsonPatchCollector : IChangeCollector
{
    private readonly JsonPatchDocument document = new JsonPatchDocument(
        new List<Operation>(),
        new CamelCasePropertyNamesContractResolver()
    );

    public void Add(ModelPath modelPath, object? value)
    {
        document.Add(GetModelPath(modelPath), value);
    }

    public void Replace(ModelPath modelPath, object? value)
    {
        document.Replace(GetModelPath(modelPath), value);
    }

    public T ReplaceIfChanged<T>(ModelPath modelPath, T original, T updated)
        where T : IEquatable<T>
    {
        if (!EqualityComparer<T>.Default.Equals(original, updated))
        {
            Replace(modelPath, updated);
        }

        return updated;
    }

    public JsonPatchDocument GetOperations() => document;

    private string GetModelPath(ModelPath path)
    {
        return string.Join(
            "",
            path.AllPathSegments()
                .Select(m =>
                {
                    switch(m)
                    {
                        case ModelPathProperty prop:
                            return $"/{ToCamelCase(prop.PropertyName)}";
                        case ModelPathArrayIndex array:
                            return $"/{array.ArrayIndex}";
                    }
                    return string.Empty;
                }));
    }

    private static string ToCamelCase(string text)
    {
        return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(text);
    }
}
