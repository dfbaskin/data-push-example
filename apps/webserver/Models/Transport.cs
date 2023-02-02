public enum TransportStatus
{
    Pending,
    Active,
    Finished
}

public record Transport(
  DateTime BeginTimestampUTC,
  DateTime? EndTimestampUTC,
  string? DriverId,
  string? VehicleId,
  Manifest Manifest,
  TransportStatus Status,
  ICollection<HistoryEntry> History
)
{
    public static Transport CreateTransport(Manifest manifest)
    {
        return new Transport(
            BeginTimestampUTC: DateTime.UtcNow,
            EndTimestampUTC: null,
            DriverId: null,
            VehicleId: null,
            Manifest: manifest,
            Status: TransportStatus.Pending,
            History: new List<HistoryEntry>()
        );
    }
}
