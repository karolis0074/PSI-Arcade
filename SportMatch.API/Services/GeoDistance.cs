namespace SportMatch.API.Services;

// distance between two points on the earth, calculated with the haversine formula
public static class GeoDistance
{
    private const double EarthRadiusKm = 6371;

    public static double Kilometers(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        double deltaLatitude = ToRadians(latitude2 - latitude1);
        double deltaLongitude = ToRadians(longitude2 - longitude1);

        double a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2)
            + Math.Cos(ToRadians(latitude1)) * Math.Cos(ToRadians(latitude2))
            * Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);

        return 2 * EarthRadiusKm * Math.Asin(Math.Min(1, Math.Sqrt(a)));
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
