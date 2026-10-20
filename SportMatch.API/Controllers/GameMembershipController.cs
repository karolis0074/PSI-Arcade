using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GameMembershipController : ControllerBase
{
    private readonly GameMembershipService _membershipService;

    public GameMembershipController(GameMembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    // POST /games/join?id=1&username=tautvydas
    [HttpPost("join")]
    public IActionResult Join([FromQuery] int id, [FromQuery] string username)
    {
        var result = _membershipService.Join(id, username);

        return result switch
        {
            MembershipResult.Success => Ok(_membershipService.GetGame(id)),
            MembershipResult.GameNotFound => NotFound(result.ToMessage()),
            _ => BadRequest(result.ToMessage())
        };
    }

    // POST /games/leave?id=1&username=tautvydas
    [HttpPost("leave")]
    public IActionResult Leave([FromQuery] int id, [FromQuery] string username)
    {
        var result = _membershipService.Leave(id, username);

        return result switch
        {
            MembershipResult.Success => Ok(_membershipService.GetGame(id)),
            MembershipResult.GameNotFound => NotFound(result.ToMessage()),
            _ => BadRequest(result.ToMessage())
        };
    }

    // GET /games/spots?id=1
    [HttpGet("spots")]
    public IActionResult GetSpots([FromQuery] int id)
    {
        var spots = _membershipService.GetSpots(id);

        if (spots is null)
            return NotFound(MembershipResult.GameNotFound.ToMessage());

        return Ok(spots);
    }
}
