public enum DriverStatus
{
    Inactive,
    Available,
    Active,
}

public record Driver(
  string Name,
  string DriverId,
  DriverStatus Status
)
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
            DriverId: $"DRV{(idx * 3) + 100:n0}",
            Status: DriverStatus.Inactive
        );
    }
}
