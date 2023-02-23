using Microsoft.AspNetCore.JsonPatch;

public record DriverDetailsView(
  string Name,
  string DriverId,
  string? GroupAssignment,
  DriverStatus Status,
  ICollection<HistoryEntry> History
)
{
    public DriverDetailsView(Driver driver)
        : this(
            Name: driver.Name,
            DriverId: driver.DriverId,
            GroupAssignment: driver.GroupAssignment,
            Status: driver.Status,
            History: driver.History
        )
    {
    }

    public static DeltasStreamUpdated InitialData(Driver driver)
    {
        return DeltasStreamUpdated.ForInitialDocument(
            DeltasStreamType.DriverDetails,
            driver.DriverId,
            new DriverDetailsView(driver)
        );
    }

    public static DeltasStreamUpdated WithDriverPatches(string driverId, JsonPatchDocument patches)
    {
        return DeltasStreamUpdated.ForPatchedDocument(
            DeltasStreamType.DriverDetails,
            driverId,
            patches.Operations
        );
    }
}
