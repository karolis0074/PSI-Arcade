using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Utils;

namespace Backend.Controllers;

[ApiController]
[Route("games")]
public class GameMembershipController : ControllerBase
{
    private readonly GameMembershipService _membership = new();
    private readonly GameStore _store = new();

    // POST /games/join?id=1&username=tautvydas
    [HttpPost("join")]
    public IActionResult Join([FromQuery] int id, [FromQuery] string username)
    {
        var result = _membership.Join(id, username);

        if (result == MembershipResult.GameNotFound)
            return NotFound(result.ToMessage());

        if (result != MembershipResult.Success)
            return BadRequest(result.ToMessage());

        return Ok(_store.GetById(id));
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

        return Ok(_store.GetById(id));
    }

    // GET /games/spots?id=1
    [HttpGet("spots")]
    public IActionResult GetSpots([FromQuery] int id)
    {
        var game = _store.GetById(id);

        if (game is null)
            return NotFound(MembershipResult.GameNotFound.ToMessage());

        var spots = new GameSpots();
        spots.Joined = game.JoinedUsernames.Count;
        spots.Max = game.MaxPlayers;
        spots.Free = _membership.GetFreeSpots(game);

        return Ok(spots);
    }
}
