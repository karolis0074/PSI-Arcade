using SportMatch.API.DataTransferObjects.Responses;
namespace SportMatch.API.Services;
public interface IGameStatisticsService
{
    Task<GameStatistics> GetOverviewAsync();
}