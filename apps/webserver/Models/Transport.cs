using RecordProxy.Generator;

public enum TransportStatus
{
    Pending,
    Loading,
    InRoute,
    Unloading,
    Returning,
    Finished
}

[GenerateProxy]
public record Transport(
    string TransportId,
    DateTime BeginTimestampUTC,
    DateTime? EndTimestampUTC,
    string? DriverId,
    string? VehicleId,
    Manifest Manifest,
    TransportStatus Status,
    ICollection<HistoryEntry> History
) : IEntityWithHistory
{
    private static int currentId = 0;
    public static Transport CreateTransport(Manifest manifest)
    {
        int id = Interlocked.Increment(ref currentId);
        return new Transport(
            TransportId: $"T-{id:d5}",
            BeginTimestampUTC: DateTime.UtcNow,
            EndTimestampUTC: null,
            DriverId: null,
            VehicleId: null,
            Manifest: manifest,
            Status: TransportStatus.Pending,
            History: new List<HistoryEntry>()
        );
    }

    public bool IsActive()
    {
        return !(
            Status == TransportStatus.Finished &&
            EndTimestampUTC != null &&
            DateTime.UtcNow.Subtract(EndTimestampUTC.Value).TotalSeconds > 20
        );
    }
}
