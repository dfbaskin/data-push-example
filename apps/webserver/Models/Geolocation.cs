using RecordProxy.Generator;

[GenerateProxy]
public record GeolocationPoint(
    double Latitude,
    double Longitude
);

[GenerateProxy]
public record GeolocationArea(
    GeolocationPoint TopLeft,
    GeolocationPoint Center,
    GeolocationPoint BottomRight
)
{
    public static GeolocationArea GetGeolocationArea()
    {
        double latOffs = SimulationWorker.LatitudeOffset / 2.0;
        double lngOffs = SimulationWorker.LongitudeOffset / 2.0;
        return new GeolocationArea(
            TopLeft: new GeolocationPoint(
                Latitude: SimulationWorker.CenterLatitude - latOffs,
                Longitude: SimulationWorker.CenterLongitude - lngOffs
            ),
            Center: new GeolocationPoint(
                Latitude: SimulationWorker.CenterLatitude,
                Longitude: SimulationWorker.CenterLongitude
            ),
            BottomRight: new GeolocationPoint(
                Latitude: SimulationWorker.CenterLatitude + latOffs,
                Longitude: SimulationWorker.CenterLongitude + lngOffs
            )
        );
    }
}

