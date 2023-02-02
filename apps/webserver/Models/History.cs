public record History(
  DateTime TimestampUTC,
  string Message
)
{
    public static History CreateHistory(string message)
    {
        return new History(
            TimestampUTC: DateTime.UtcNow,
            Message: message
        );
    }
}
