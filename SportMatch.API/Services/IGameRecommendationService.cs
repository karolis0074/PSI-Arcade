using SportMatch.API.DataTransferObjects.Responses;

namespace SportMatch.API.Services;

public interface IGameRecommendationService
{
    public IReadOnlyList<RecommendedGame> GetRecommended(string username, double? latitude = null, double? longitude = null);
}
