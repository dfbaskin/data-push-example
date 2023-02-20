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
        double latOffs = TransportsSimulation.LatitudeOffset / 2.0;
        double lngOffs = TransportsSimulation.LongitudeOffset / 2.0;
        return new GeolocationArea(
            TopLeft: new GeolocationPoint(
                Latitude: TransportsSimulation.CenterLatitude - latOffs,
                Longitude: TransportsSimulation.CenterLongitude - lngOffs
            ),
            Center: new GeolocationPoint(
                Latitude: TransportsSimulation.CenterLatitude,
                Longitude: TransportsSimulation.CenterLongitude
            ),
            BottomRight: new GeolocationPoint(
                Latitude: TransportsSimulation.CenterLatitude + latOffs,
                Longitude: TransportsSimulation.CenterLongitude + lngOffs
            )
        );
    }
}

