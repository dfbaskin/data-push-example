using Microsoft.AspNetCore.JsonPatch;

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
        return DeltasStreamUpdated.ForInitialDocument(
            DeltasStreamType.TransportDetails,
            transport.TransportId,
            new TransportDetailsView(transport, driver, vehicle)
        );
    }

    public static DeltasStreamUpdated WithTransportPatches(string transportId, JsonPatchDocument patches)
    {
        return DeltasStreamUpdated.ForPatchedDocument(
            DeltasStreamType.TransportDetails,
            transportId,
            patches
                .FilterPatches(
                    nameof(TransportId),
                    nameof(Status),
                    nameof(BeginTimestampUTC),
                    nameof(EndTimestampUTC),
                    nameof(Manifest),
                    nameof(History)
                )
        );
    }

    public static DeltasStreamUpdated WithDriverPatches(string transportId, string driverId, JsonPatchDocument patches)
    {
        return DeltasStreamUpdated.ForPatchedDocument(
            DeltasStreamType.TransportDetails,
            transportId,
            patches
                .FilterPatches(
                    nameof(TransportDetailsDriverView.DriverId),
                    nameof(TransportDetailsDriverView.Name),
                    nameof(TransportDetailsDriverView.GroupAssignment),
                    nameof(TransportDetailsDriverView.Status)
                )
                .ApplyPrefix(nameof(Driver))
        );
    }

    public static DeltasStreamUpdated WithVehiclePatches(string transportId, string vehicleId, JsonPatchDocument patches)
    {
        return DeltasStreamUpdated.ForPatchedDocument(
            DeltasStreamType.TransportDetails,
            transportId,
            patches
                .FilterPatches(
                    nameof(TransportDetailsVehicleView.VehicleId),
                    nameof(TransportDetailsVehicleView.VehicleType),
                    nameof(TransportDetailsVehicleView.Status),
                    nameof(TransportDetailsVehicleView.Location)
                )
                .ApplyPrefix(nameof(Vehicle))
        );
    }
}
