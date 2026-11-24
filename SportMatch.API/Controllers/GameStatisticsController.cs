using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GameStatisticsController : ControllerBase
{
    private readonly IGameStatisticsService _statisticsService;

    public GameStatisticsController(IGameStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    // GET /games/statistics
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics() =>
        Ok(await _statisticsService.GetOverviewAsync());
}
