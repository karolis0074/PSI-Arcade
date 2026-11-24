using System.Globalization;
using SportMatch.API.Models;

namespace SportMatch.API.Extensions;

public static class SportFieldExtensions
{
    // in sportfields.json the address is stored as "latitude, longitude"
    public static bool TryGetCoordinates(this SportField field, out double latitude, out double longitude)
    {
        latitude = 0;
        longitude = 0;

        var parts = field.Address.Split(',');
        if (parts.Length != 2)
            return false;

        if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out latitude)
            || !double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
            return false;

        return latitude is >= -90 and <= 90 && longitude is >= -180 and <= 180;
    }
}
