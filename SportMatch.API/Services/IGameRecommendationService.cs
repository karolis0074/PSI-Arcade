using SportMatch.API.DataTransferObjects.Responses;

namespace SportMatch.API.Services;

public interface IGameRecommendationService
{
    Task<IReadOnlyList<RecommendedGame>> GetRecommendedAsync(string username, double? latitude = null, double? longitude = null);
}
