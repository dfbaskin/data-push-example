using RecordProxy.Generator;

[GenerateProxy]
public record Manifest(
  DateTime CreatedTimestampUTC,
  ICollection<ManifestItem> Items,
  ICollection<HistoryEntry> History
)
{
    public static Manifest CreateManifest()
    {
        return new Manifest(
            CreatedTimestampUTC: DateTime.UtcNow,
            Items: Enumerable
                .Range(1, Faker.RandomNumber.Next(1, 5))
                .Select(_ => ManifestItem.CreateManifestItem())
                .ToList(),
            History: new List<HistoryEntry>
            {
                HistoryEntry.CreateHistoryEntry("Created manifest.")
            }
        );
    }
}

[GenerateProxy]
public record ManifestItem(
    string ItemId,
    int Quantity,
    string Description
)
{
    public static ManifestItem CreateManifestItem()
    {
        return new ManifestItem(
            ItemId: Faker.Model.ProductId(),
            Quantity: Faker.RandomNumber.Next(1, 5),
            Description: Faker.Model.Description()
        );
    }
}
