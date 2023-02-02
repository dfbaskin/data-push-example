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

    public static T PickOneOf<T>(this IEnumerable<T> items)
        where T : class
    {
        var list = items.ToList();
        return list.ElementAt(Faker.RandomNumber.Next(list.Count - 1));
    }

    public static ICollection<T> AppendItem<T>(this IEnumerable<T> items, T item)
    {
        return items.Append(item).ToList();
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
