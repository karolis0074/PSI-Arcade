using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;

namespace SportMatch.API.Services
{
    public interface IGameStatisticsService
    {
        public GameStatistics GetOverview();
    }
}
