namespace Faker;

public static class Model
{
    public static string ProductId()
    {
        return string.Join(
            "-",
            Enumerable
                .Range(0, 3)
                .Select(_ => Faker.RandomNumber.Next(1, 9999))
                .Select(value => $"{value:d}")
        );
    }

    public static string Description(int words = 6)
    {
        return Faker.Lorem.Words(words).AsCapitalizedString();
    }

    public static string ItemId(string prefix, int value)
    {
        return $"{prefix}{value:d3}";
    }
}
