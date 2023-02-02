public static class FakerExtensions
{
    public static string AsConcatenatedString(this IEnumerable<string> strings)
    {
        return string.Join(" ", strings);
    }

    public static string AsCapitalizedString(this IEnumerable<string> strings)
    {
        return string.Join(
            " ",
            strings.Select((s, idx) => idx == 0 ? Capitalize(s) : s)
        );
    }

    private static string Capitalize(string value)
    {
        if (value.Length == 0)
        {
            return value;
        }
        return string.Concat(value[0].ToString().ToUpper(), value.AsSpan(1));
    }
}
