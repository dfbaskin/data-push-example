public class Query
{
    private CurrentData Current { get; }

    public Query(CurrentData current)
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
    }

    public IEnumerable<Group> GetGroups() => Current.Groups;
    public IEnumerable<Driver> GetDrivers() => Current.Drivers;
    public IEnumerable<Vehicle> GetVehicles() => Current.Vehicles;
    public IEnumerable<Transport> GetAllTransports() =>
        Current.Transports;
    public IEnumerable<Transport> GetActiveTransports() =>
        Current.Transports.Where(t => t.IsActive());
    public Configuration GetConfiguration() => Current.Configuration;

    public Driver? GetDriver(string driverId) =>
        Current.Drivers
            .Where(t => Matches(t.DriverId, driverId))
            .FirstOrDefault();
    public Vehicle? GetVehicle(string vehicleId) =>
        Current.Vehicles
            .Where(t => Matches(t.VehicleId, vehicleId))
            .FirstOrDefault();
    public Transport? GetTransport(string transportId) =>
        Current.Transports
            .Where(t => Matches(t.TransportId, transportId))
            .FirstOrDefault();

    private bool Matches(string a, string b)
    {
        return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
    }
}
