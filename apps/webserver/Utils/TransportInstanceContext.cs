internal record TransportInstanceContext(
    CancellationToken Token,
    string GroupAssignment,
    Transport Transport,
    Driver Driver,
    Vehicle Vehicle
) : SimulationContext(
    Token: Token
)
{
    public string DriverId => Driver.DriverId;
    public string VehicleId => Vehicle.VehicleId;
    public string TransportId => Transport.TransportId;
    public Manifest Manifest => Transport.Manifest;
}
