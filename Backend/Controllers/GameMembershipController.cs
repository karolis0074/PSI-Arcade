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
}
