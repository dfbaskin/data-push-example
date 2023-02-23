using System.Text.Json;

public record TransportDetailsManifestView(
    DateTime CreatedTimestampUTC,
    ICollection<ManifestItem> Items
)
{
    public TransportDetailsManifestView(Manifest manifest)
        : this(
            CreatedTimestampUTC: manifest.CreatedTimestampUTC,
            Items: manifest.Items
        )
    {
    }
}

public record TransportDetailsDriverView(
    string DriverId,
    string Name,
    string? GroupAssignment,
    DriverStatus Status
)
{
    public TransportDetailsDriverView(Driver driver)
        : this(
            DriverId: driver.DriverId,
            Name: driver.Name,
            GroupAssignment: driver.GroupAssignment,
            Status: driver.Status
        )
    {
    }
}

public record TransportDetailsVehicleView(
    string VehicleId,
    string VehicleType,
    VehicleStatus Status,
    Location Location
)
{
    public TransportDetailsVehicleView(Vehicle vehicle)
        : this(
            VehicleId: vehicle.VehicleId,
            VehicleType: vehicle.VehicleType,
            Status: vehicle.Status,
            Location: vehicle.Location
        )
    {
    }
}

public record TransportDetailsView(
    string TransportId,
    TransportStatus Status,
    DateTime BeginTimestampUTC,
    DateTime? EndTimestampUTC,
    TransportDetailsManifestView Manifest,
    TransportDetailsDriverView Driver,
    TransportDetailsVehicleView Vehicle,
    ICollection<HistoryEntry> History
)
{
    public TransportDetailsView(Transport transport, Driver driver, Vehicle vehicle)
        : this(
            TransportId: transport.TransportId,
            Status: transport.Status,
            BeginTimestampUTC: transport.BeginTimestampUTC,
            EndTimestampUTC: transport.EndTimestampUTC,
            Manifest: new TransportDetailsManifestView(transport.Manifest),
            Driver: new TransportDetailsDriverView(driver),
            Vehicle: new TransportDetailsVehicleView(vehicle),
            History: transport.History
        )
    {
    }

    public static DeltasStreamUpdated InitialData(Transport transport, Driver driver, Vehicle vehicle)
    {
        return new DeltasStreamUpdated(
            StreamType: DeltasStreamType.TransportDetails,
            Id: transport.TransportId,
            JsonDocument: JsonSerializer.Serialize<TransportDetailsView>(
                new TransportDetailsView(transport, driver, vehicle)
            )
        );
    }
}
