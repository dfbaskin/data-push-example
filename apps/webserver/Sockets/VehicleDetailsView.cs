using Microsoft.AspNetCore.JsonPatch;

public record VehicleDetailsView(
  string VehicleType,
  string VehicleId,
  VehicleStatus Status,
  Location Location,
  ICollection<HistoryEntry> History
)
{
    public VehicleDetailsView(Vehicle vehicle)
        : this(
            VehicleType: vehicle.VehicleType,
            VehicleId: vehicle.VehicleId,
            Status: vehicle.Status,
            Location: vehicle.Location,
            History: vehicle.History
        )
    {
    }

    public static DeltasStreamUpdated InitialData(Vehicle vehicle)
    {
        return DeltasStreamUpdated.ForInitialDocument(
            DeltasStreamType.VehicleDetails,
            vehicle.VehicleId,
            new VehicleDetailsView(vehicle)
        );
    }

    public static DeltasStreamUpdated WithVehiclePatches(string vehicleId, JsonPatchDocument patches)
    {
        return DeltasStreamUpdated.ForPatchedDocument(
            DeltasStreamType.VehicleDetails,
            vehicleId,
            patches.Operations
        );
    }
}
