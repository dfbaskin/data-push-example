using RecordProxy.Generator;

public enum DriverStatus
{
    Inactive,
    Available,
    Active,
}

[GenerateProxy]
public record Driver(
  string Name,
  string DriverId,
  string? GroupAssignment,
  DriverStatus Status,
  ICollection<HistoryEntry> History
) : IEntityWithHistory
{
    public const int DriverCount = 15;

    public static IEnumerable<Driver> CreateDrivers()
    {
        for(int i = 0; i < DriverCount; i++) {
            yield return CreateDriver(i + 1);
        }
    }

    public static Driver CreateDriver(int idx)
    {
        return new Driver(
            Name: Faker.Name.FullName(),
            DriverId: Faker.Model.ItemId("DRV", idx * 3),
            GroupAssignment: null,
            Status: idx == 7 ?  DriverStatus.Inactive : DriverStatus.Available,
            History: new List<HistoryEntry>()
        );
    }
}
