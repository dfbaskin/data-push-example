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
    public IEnumerable<Transport> GetTransports() => Current.Transports;
    public Configuration GetConfiguration() => Current.Configuration;
}
