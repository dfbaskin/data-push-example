
[ExtendObjectType(typeof(Vehicle))]
public class VehicleExtensions
{
    private CurrentData Current { get; }

    public VehicleExtensions(CurrentData current)
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
    }

    public Transport? GetTransport([Parent] Vehicle vehicle)
    {
        return Current.Transports
            .Where(t => t.VehicleId == vehicle.VehicleId && t.Status != TransportStatus.Finished)
            .FirstOrDefault();
    }
}
