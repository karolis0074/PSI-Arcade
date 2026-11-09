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
    private readonly GameMembershipService _membershipService;

    public GameMembershipController(GameMembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    private string GetUsername() => User.Identity!.Name!;

    // POST /games/1/players
    [Authorize]
    [HttpPost("{id}/players")]
    public IActionResult Join(int id)
    {
        var result = _membershipService.Join(id, GetUsername());

        return result switch
        {
            MembershipResult.Success => Ok(_membershipService.GetGame(id)),
            MembershipResult.GameNotFound => NotFound(result.ToMessage()),
            _ => BadRequest(result.ToMessage())
        };
    }

    // DELETE /games/1/players/me
    [Authorize]
    [HttpDelete("{id}/players/me")]
    public IActionResult Leave(int id)
    {
        var result = _membershipService.Leave(id, GetUsername());

        return result switch
        {
            MembershipResult.Success => Ok(_membershipService.GetGame(id)),
            MembershipResult.GameNotFound => NotFound(result.ToMessage()),
            _ => BadRequest(result.ToMessage())
        };
    }

    // GET /games/1/spots
    [HttpGet("{id}/spots")]
    public IActionResult GetSpots(int id)
    {
        var spots = _membershipService.GetSpots(id);

        if (spots is null)
            return NotFound(MembershipResult.GameNotFound.ToMessage());

        return Ok(spots);
    }
}
