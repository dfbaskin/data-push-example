
[ExtendObjectType(typeof(Driver))]
public class DriverExtensions
{
    private CurrentData Current { get; }

    public DriverExtensions(CurrentData current)
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
    }

    public Transport? GetTransport([Parent] Driver driver)
    {
        return Current.Transports
            .Where(t => t.DriverId == driver.DriverId && t.Status != TransportStatus.Finished)
            .FirstOrDefault();
    }
}
