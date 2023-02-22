using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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

    private class UpperSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
            => name.ToUpperSnakeCase();
    }
}
