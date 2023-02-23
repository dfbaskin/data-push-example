using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

public static class JsonUtils
{
    public static JsonSerializerOptions SerializerOptions { get; }

    static JsonUtils()
    {
        SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        SerializerOptions.Converters.Add(
            new JsonStringEnumConverter(
                new UpperSnakeCaseNamingPolicy()
            )
        );
    }

    // Matches the GraphQL recommendation for Enum values.
    public static string ToUpperSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return string.Join(
            string.Empty,
            value.Select((ch, idx) =>
            {
                var sep = (idx > 0 && char.IsUpper(ch)) ? "_" : string.Empty;
                return $"{sep}{char.ToUpper(ch)}";
            }));
    }

    public static string ToCamelCase(this string value)
    {
        return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(value);
    }


    public static IEnumerable<Operation> FilterPatches(
        this JsonPatchDocument patches,
        params string[] includePropertyNames
    )
    {
        var propNames = includePropertyNames
            .Select(ConvertPropertyPath)
            .ToList();

        foreach (var operation in patches.Operations)
        {
            if (propNames.Any(name => operation.path.StartsWith(name)))
            {
                yield return operation;
            }
        }
    }

    public static IEnumerable<Operation> ApplyPrefix(
        this IEnumerable<Operation> operations,
        string parentPropertyName
    )
    {
        var prefix = ConvertPropertyPath(parentPropertyName);
        foreach (var operation in operations)
        {
            yield return new Operation(
                op: operation.op,
                path: $"{prefix}{operation.path}",
                from: operation.from,
                value: operation.value
            );
        }
    }

    public static string ConvertPropertyPath(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        return string.Join("/", ConvertPropertyPathSegments(name));
    }

    private static IEnumerable<string> ConvertPropertyPathSegments(string name)
    {
        yield return string.Empty;
        foreach (var segment in name.Split('/'))
        {
            yield return segment.ToCamelCase();
        }
    }

    private class UpperSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
            => name.ToUpperSnakeCase();
    }
}
