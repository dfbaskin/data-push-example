using RecordProxy.Generator;

[GenerateProxy]
public record HistoryEntry(
  DateTime TimestampUTC,
  string Message
)
{
    public static HistoryEntry CreateHistoryEntry(string message)
    {
        return new HistoryEntry(
            TimestampUTC: DateTime.UtcNow,
            Message: message
        );
    }
}

public interface IEntityWithHistory
{
    ICollection<HistoryEntry> History { get; }
}
