public record DeltasStreamRequest(
    DeltasStreamType StreamType,
    bool Subscribe,
    string? Id = null
)
{
    public bool Matches(DeltasStreamUpdated other)
    {
        switch (StreamType)
        {
            case DeltasStreamType.Groups:
            case DeltasStreamType.Geolocation:
            case DeltasStreamType.Transports:
                return StreamType == other.StreamType;
            case DeltasStreamType.TransportDetails:
            case DeltasStreamType.VehicleDetails:
            case DeltasStreamType.DriverDetails:
                return StreamType == other.StreamType && Id == other.Id;
        }

        return false;
    }

    public bool ShouldReplace(DeltasStreamRequest other)
    {
        switch (StreamType)
        {
            case DeltasStreamType.Groups:
            case DeltasStreamType.Geolocation:
            case DeltasStreamType.Transports:
                return StreamType == other.StreamType;
            case DeltasStreamType.TransportDetails:
            case DeltasStreamType.VehicleDetails:
            case DeltasStreamType.DriverDetails:
                return StreamType != other.StreamType || Id != other.Id;
        }

        return false;
    }
}
