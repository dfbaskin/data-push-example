public class CurrentData
{
    public IReadOnlyCollection<Group> Groups { get; } =
        Group.CreateGroups().ToList();

    public IReadOnlyCollection<Driver> Drivers { get; } =
        Driver.CreateDrivers().ToList();

    public IReadOnlyCollection<Vehicle> Vehicles { get; } =
        Vehicle.CreateVehicles().ToList();
}
