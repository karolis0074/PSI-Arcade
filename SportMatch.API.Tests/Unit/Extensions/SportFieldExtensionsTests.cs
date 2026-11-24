using System.Globalization;
using SportMatch.API.Extensions;
using SportMatch.API.Models;

namespace SportMatch.API.Tests.Unit.Extensions;

public class SportFieldExtensionsTests
{
    private static SportField FieldAt(string address) => new() { Address = address };

    [Fact]
    public void TryGetCoordinates_CoordinatesAddress_ReturnsTrueAndValues()
    {
        var ok = FieldAt("54.7385459, 25.2377033").TryGetCoordinates(out var latitude, out var longitude);

        Assert.True(ok);
        Assert.Equal(54.7385459, latitude, 7);
        Assert.Equal(25.2377033, longitude, 7);
    }

    [Theory]
    [InlineData("Gedimino pr. 9")]
    [InlineData("")]
    [InlineData("54.73")]
    [InlineData("54.73, 25.23, 10")]
    [InlineData("abc, def")]
    [InlineData("91, 25.23")]
    [InlineData("54.73, 181")]
    public void TryGetCoordinates_NotValidCoordinates_ReturnsFalse(string address)
    {
        var ok = FieldAt(address).TryGetCoordinates(out _, out _);

        Assert.False(ok);
    }

    [Fact]
    public void TryGetCoordinates_LithuanianCulture_StillReadsDotAsDecimalSeparator()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("lt-LT");

            var ok = FieldAt("54.5, 25.5").TryGetCoordinates(out var latitude, out var longitude);

            Assert.True(ok);
            Assert.Equal(54.5, latitude);
            Assert.Equal(25.5, longitude);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
