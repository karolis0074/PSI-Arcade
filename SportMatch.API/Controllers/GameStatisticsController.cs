using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GameStatisticsController : ControllerBase
{
    private readonly GameStatisticsService _statistics;

    public GameStatisticsController(GameStatisticsService statistics)
    {
        _statistics = statistics;
    }

    // GET /games/statistics
    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        return Ok(_statistics.GetOverview());
    }
}
