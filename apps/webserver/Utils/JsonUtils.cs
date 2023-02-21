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
        if(string.IsNullOrEmpty(value))
        {
            return value;
        }

        return SnakeCaseRegex.Replace(value, m => $"_{m.Value}").ToUpperInvariant();
    }

    private static Regex SnakeCaseRegex = new Regex(@"[a-z]");

    private class UpperSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
            => name.ToUpperSnakeCase();
    }
}
