public enum VehicleStatus
{
    OutOfService,
    Available,
    Active
}

public record Vehicle(
  string VehicleType,
  string VehicleId,
  VehicleStatus Status,
  Location? Location,
  ICollection<HistoryEntry> History
) : IEntityWithHistory
{
    private const int VehicleCount = 10;

    private static List<string> VehicleTypes =
        new List<string>
        {
            "Truck",
            "Van"
        };

    public static IEnumerable<Vehicle> CreateVehicles()
    {
        for(int i = 0; i < VehicleCount; i++) {
            yield return CreateVehicle(i + 1);
        }
    }

    public static Vehicle CreateVehicle(int idx)
    {
        return new Vehicle(
            VehicleType: VehicleTypes.PickOneOf(),
            VehicleId: Faker.Model.ItemId("VEH", idx * 4),
            Status: idx == 3 ? VehicleStatus.OutOfService : VehicleStatus.Available,
            Location: new Location(
                Latitude: null,
                Longitude: null,
                Address: null
            ),
            History: new List<HistoryEntry>()
        );
    }
}
