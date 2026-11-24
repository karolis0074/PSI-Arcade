using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GameRecommendationsController : ControllerBase
{
    private readonly IGameRecommendationService _recommendationService;

    public GameRecommendationsController(IGameRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    private string GetUsername() => User.Identity!.Name!;

    // GET /games/recommended?lat=54.68&lng=25.28
    [Authorize]
    [HttpGet("recommended")]
    public IActionResult GetRecommended([FromQuery] double? lat, [FromQuery] double? lng)
    {
        if ((lat is null) != (lng is null))
            return BadRequest("Both lat and lng are required");

        if (lat is < -90 or > 90 || lng is < -180 or > 180)
            return BadRequest("Invalid location");

        return Ok(_recommendationService.GetRecommended(GetUsername(), lat, lng));
    }
}
