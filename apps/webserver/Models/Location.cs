using RecordProxy.Generator;

[GenerateProxy]
public record Location(
    double? Latitude,
    double? Longitude,
    string? Address
);
