using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GameStatisticsController : ControllerBase
{
    private readonly GameStatisticsService _statisticsService;

    public GameStatisticsController(GameStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    // GET /games/statistics
    [HttpGet("statistics")]
    public IActionResult GetStatistics() => Ok(_statisticsService.GetOverview());
}
