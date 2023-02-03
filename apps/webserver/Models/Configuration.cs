
public record Configuration(
    GeolocationArea Area
)
{
    public static Configuration GetConfiguration()
    {
        return new Configuration(
            Area: GeolocationArea.GetGeolocationArea()
        );
    }
}
