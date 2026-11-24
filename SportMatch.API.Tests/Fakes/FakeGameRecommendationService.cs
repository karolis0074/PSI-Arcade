using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Services;

namespace SportMatch.API.Tests.Fakes;

// fake service for controller tests: returns a list set by the test
// and remembers what the controller passed in
public class FakeGameRecommendationService : IGameRecommendationService
{
    public List<RecommendedGame> Result { get; } = new();

    public bool WasCalled { get; private set; }
    public string? LastUsername { get; private set; }
    public double? LastLatitude { get; private set; }
    public double? LastLongitude { get; private set; }

    public Task<IReadOnlyList<RecommendedGame>> GetRecommendedAsync(string username, double? latitude = null, double? longitude = null)
    {
        WasCalled = true;
        LastUsername = username;
        LastLatitude = latitude;
        LastLongitude = longitude;
        return Task.FromResult<IReadOnlyList<RecommendedGame>>(Result);
    }
}
