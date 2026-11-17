using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("metrics")]
public class MetricsController : ControllerBase
{
    private readonly LiveMetricsService _metrics;

    public MetricsController(LiveMetricsService metrics)
    {
        _metrics = metrics;
    }

    [HttpGet("active-users")]
    public IActionResult GetActiveUsers()
    {
        return Ok(new { activeUsers = _metrics.GetActiveUserCount() });
    }
}