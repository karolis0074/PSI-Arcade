using SportMatch.API.Data;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GameMembershipController : ControllerBase
{
    private readonly GameMembershipService _membership;

    public GameMembershipController(GameMembershipService membership)
    {
        _membership = membership;
    }

    // POST /games/join?id=1&username=tautvydas
    [HttpPost("join")]
    public IActionResult Join([FromQuery] int id, [FromQuery] string username)
    {
        var result = _membership.Join(id, username);

        if (result == MembershipResult.GameNotFound)
            return NotFound(result.ToMessage());

        if (result != MembershipResult.Success)
            return BadRequest(result.ToMessage());

        return Ok(_membership.GetGame(id));
    }

    // POST /games/leave?id=1&username=tautvydas
    [HttpPost("leave")]
    public IActionResult Leave([FromQuery] int id, [FromQuery] string username)
    {
        var result = _membership.Leave(id, username);

        if (result == MembershipResult.GameNotFound)
            return NotFound(result.ToMessage());

        if (result != MembershipResult.Success)
            return BadRequest(result.ToMessage());

        return Ok(_membership.GetGame(id));
    }

    // GET /games/spots?id=1
    [HttpGet("spots")]
    public IActionResult GetSpots([FromQuery] int id)
    {
        var spots = _membership.GetSpots(id);

        if (spots is null)
            return NotFound(MembershipResult.GameNotFound.ToMessage());

        return Ok(spots);
    }
}