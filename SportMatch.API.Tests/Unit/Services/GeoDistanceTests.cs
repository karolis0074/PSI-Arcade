using SportMatch.API.Services;

namespace SportMatch.API.Tests.Unit.Services;

public class GeoDistanceTests
{
    [Fact]
    public void Kilometers_SamePoint_ReturnsZero()
    {
        var distance = GeoDistance.Kilometers(54.68, 25.28, 54.68, 25.28);

        Assert.Equal(0.0, distance, 6);
    }

    [Fact]
    public void Kilometers_VilniusToKaunas_IsAbout93Km()
    {
        var distance = GeoDistance.Kilometers(54.6858, 25.2877, 54.8972, 23.8861);

        Assert.InRange(distance, 92.5, 93.3);
    }

    [Fact]
    public void Kilometers_IsTheSameInBothDirections()
    {
        var there = GeoDistance.Kilometers(54.68, 25.28, 54.90, 23.89);
        var back = GeoDistance.Kilometers(54.90, 23.89, 54.68, 25.28);

        Assert.Equal(there, back, 6);
    }
}
