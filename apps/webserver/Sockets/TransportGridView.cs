using Microsoft.AspNetCore.JsonPatch;

public record TransportGridView(
    string TransportId,
    TransportStatus TransportStatus,
    string VehicleId,
    string VehicleType,
    double? Latitude,
    double? Longitude,
    string? Address,
    string DriverId,
    DriverStatus DriverStatus
)
{
    public TransportGridView(Transport transport, Driver driver, Vehicle vehicle)
        : this(
            TransportId: transport.TransportId,
            TransportStatus: transport.Status,
            VehicleId: vehicle.VehicleId,
            VehicleType: vehicle.VehicleType,
            Latitude: vehicle.Location.Latitude,
            Longitude: vehicle.Location.Longitude,
            Address: vehicle.Location.Address,
            DriverId: driver.DriverId,
            DriverStatus: driver.Status
        )
    {
    }

    public static DeltasStreamUpdated InitialData(
        ICollection<Transport> transports,
        ICollection<Driver> drivers,
        ICollection<Vehicle> vehicles
    )
    {
        var activeTransports = transports
            .Where(transport => transport.Status != TransportStatus.Finished)
            .Select(transport =>
            {
                var driver = drivers.FirstOrDefault(v => v.DriverId == transport.DriverId);
                var vehicle = vehicles.FirstOrDefault(v => v.VehicleId == transport.VehicleId);
                return new TransportGridView
                (
                    TransportId: transport.TransportId,
                    TransportStatus: transport.Status,
                    VehicleId: vehicle?.VehicleId ?? "?",
                    VehicleType: vehicle?.VehicleType ?? "?",
                    Latitude: vehicle?.Location?.Latitude,
                    Longitude: vehicle?.Location?.Longitude,
                    Address: vehicle?.Location?.Address,
                    DriverId: driver?.DriverId ?? "?",
                    DriverStatus: driver?.Status ?? DriverStatus.Inactive
                );
            })
            .ToList();
        return DeltasStreamUpdated.ForInitialDocument(
            DeltasStreamType.Transports,
            null,
            activeTransports
        );
    }

    public static DeltasStreamUpdated WithTransportPatches(string transportId, JsonPatchDocument patches)
    {
        return DeltasStreamUpdated.ForPatchedDocument(
            DeltasStreamType.Transports,
            transportId,
            patches
                .FilterPatches(
                    nameof(TransportId),
                    nameof(TransportStatus)
                )
                .ChangePropertyName(
                    nameof(Transport.Status),
                    nameof(TransportStatus)
                )
        );
    }

    public static DeltasStreamUpdated WithDriverPatches(string transportId, string driverId, JsonPatchDocument patches)
    {
        return DeltasStreamUpdated.ForPatchedDocument(
            DeltasStreamType.Transports,
            transportId,
            patches
                .FilterPatches(
                    nameof(Driver.DriverId),
                    nameof(Driver.Status)
                )
                .ChangePropertyName(
                    nameof(Driver.Status),
                    nameof(DriverStatus)
                )
        );
    }

    public static DeltasStreamUpdated WithVehiclePatches(string transportId, string vehicleId, JsonPatchDocument patches)
    {
        return DeltasStreamUpdated.ForPatchedDocument(
            DeltasStreamType.Transports,
            transportId,
            patches
                .FilterPatches(
                    nameof(Vehicle.VehicleId),
                    nameof(Vehicle.VehicleType),
                    nameof(Vehicle.Location)
                )
                .ChangePropertyName(
                    $"{nameof(Vehicle.Location)}/{nameof(Location.Latitude)}",
                    nameof(Latitude)
                )
                .ChangePropertyName(
                    $"{nameof(Vehicle.Location)}/{nameof(Location.Longitude)}",
                    nameof(Longitude)
                )
                .ChangePropertyName(
                    $"{nameof(Vehicle.Location)}/{nameof(Location.Address)}",
                    nameof(Address)
                )
        );
    }
}
