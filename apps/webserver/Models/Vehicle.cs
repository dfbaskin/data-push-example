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
  ICollection<History> History
)
{
    private const int VehicleCount = 10;
    private const double CenterLatitude = 35.565752687910056;
    private const double CenterLongitude = -83.49854631914549;
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
            VehicleType: VehicleTypes.ElementAt(Faker.RandomNumber.Next(VehicleTypes.Count - 1)),
            VehicleId: $"VH{(idx * 4) + 100:n0}",
            Status: idx == 3 ? VehicleStatus.OutOfService : VehicleStatus.Available,
            Location: new Location(
                Latitude: CenterLatitude,
                Longitude: CenterLongitude,
                Address: null
            ),
            History: new List<History>()
        );
    }
}
