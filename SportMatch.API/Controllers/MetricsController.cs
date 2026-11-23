using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("metrics")]
public class MetricsController : ControllerBase
{
    private readonly ILiveMetricsService _metrics;

    public MetricsController(ILiveMetricsService metrics)
    {
        _metrics = metrics;
    }

    // GET /metrics/active-users
    [HttpGet("active-users")]
    public IActionResult GetActiveUsers()
    {
        return Ok(new { activeUsers = _metrics.GetActiveUserCount() });
    }
}