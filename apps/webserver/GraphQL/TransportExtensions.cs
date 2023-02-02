
[ExtendObjectType(typeof(Transport))]
public class TransportExtensions
{
    private CurrentData Current { get; }

    public TransportExtensions(CurrentData current)
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
    }

    public Driver? GetDriver([Parent] Transport transport)
    {
        return Current.Drivers
            .Where(d => d.DriverId == transport.DriverId)
            .FirstOrDefault();
    }

    public Vehicle? GetVehicle([Parent] Transport transport)
    {
        return Current.Vehicles
            .Where(v => v.VehicleId == transport.VehicleId)
            .FirstOrDefault();
    }
}
