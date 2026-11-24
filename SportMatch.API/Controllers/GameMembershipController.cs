using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Extensions;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GameMembershipController : ControllerBase
{
    private readonly IGameMembershipService _membership;

    public GameMembershipController(IGameMembershipService membership)
    {
        _membership = membership;
    }

    // the username always comes from the JWT token, so nobody can join or leave as someone else
    private string GetUsername() => User.Identity!.Name!;

    // POST /games/1/players
    [Authorize]
    [HttpPost("{id:int}/players")]
    public async Task<IActionResult> Join(int id)
    {
        var result = await _membership.JoinAsync(id, GetUsername());

        if (result == MembershipResult.GameNotFound)
            return NotFound(result.ToMessage());
        if (result != MembershipResult.Success)
            return BadRequest(result.ToMessage());

        return Ok(await _membership.GetGameAsync(id));
    }

    // DELETE /games/1/players/me
    [Authorize]
    [HttpDelete("{id:int}/players/me")]
    public async Task<IActionResult> Leave(int id)
    {
        var result = await _membership.LeaveAsync(id, GetUsername());

        if (result == MembershipResult.GameNotFound)
            return NotFound(result.ToMessage());
        if (result != MembershipResult.Success)
            return BadRequest(result.ToMessage());

        return Ok(await _membership.GetGameAsync(id));
    }

    // DELETE /games/1/players/jonas
    // ("/players/me" above is a fixed route, so it always goes to Leave, not here)
    [Authorize]
    [HttpDelete("{id:int}/players/{username}")]
    public async Task<IActionResult> Kick(int id, string username)
    {
        var result = await _membership.KickAsync(id, GetUsername(), username);

        return result switch
        {
            MembershipResult.Success => Ok(await _membership.GetGameAsync(id)),
            MembershipResult.GameNotFound => NotFound(result.ToMessage()),
            MembershipResult.NotHost => Forbid(),   // 403
            _ => BadRequest(result.ToMessage())
        };
    }

    // GET /games/1/spots
    [HttpGet("{id:int}/spots")]
    public async Task<IActionResult> GetSpots(int id)
    {
        var spots = await _membership.GetSpotsAsync(id);

        if (spots is null)
            return NotFound(MembershipResult.GameNotFound.ToMessage());

        return Ok(spots);
    }
}
