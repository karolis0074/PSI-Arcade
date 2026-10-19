using SportMatch.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Services;
using SportMatch.API.DataTransferObjects.Requests;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GamesController : ControllerBase
{
    private string GetUsername() => User.Identity!.Name!;
    private readonly GameRepository _store;
    private readonly AppDbContext _db;
    private readonly GameManagerService _gameManager;
    private readonly GameMembershipService _membership;

    public GamesController(GameRepository store, AppDbContext db, GameManagerService gameManager, GameMembershipService membership)
    {
        _store = store;
        _db = db;
        _gameManager = gameManager;
        _membership = membership;
    }

    // GET /games/getall
    [HttpGet("getall")]
    public IActionResult GetAll()
    {
        return Ok(_store.GetAll());
    }

    // GET /games/get?id=1
    [HttpGet("get")]
    public IActionResult GetById([FromQuery] int id)
    {
        var game = _store.GetById(id);

        if (game is null)
            return NotFound();

        return Ok(game);
    }

    // POST /games/create
    [Authorize]
    [HttpPost("create")]
        public IActionResult Create(CreateGameRequest request)
    {
        if (request.StartTime <= DateTime.UtcNow)
            return BadRequest("StartTime must be in the future");

        var field = _db.SportFields.FirstOrDefault(f => f.Id == request.FootballFieldId);
        if (field is null)
            return BadRequest("Sport field not found");

        if (request.MaxPlayers > field.Capacity)
            return BadRequest($"MaxPlayers cannot exceed the field capacity ({field.Capacity})");

        var username = GetUsername();

        var game = new Game
        {
            FootballFieldId = request.FootballFieldId,
            StartTime = request.StartTime,
            MaxPlayers = request.MaxPlayers,
            CreatedByUsername = username
        };
        game.JoinedUsernames.Add(username);
        _membership.UpdateStatus(game);

        var created = _store.Add(game);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // DELETE /games/delete?id=1
    [Authorize]
    [HttpDelete("delete")]
    public IActionResult Delete([FromQuery] int id)
    {
        var game = _store.GetById(id);

        if (game is null)
            return NotFound();

        if (!_gameManager.IsHost(GetUsername(), game))
            return Forbid();   // 403

        var success = _store.Delete(id);

        if (!success)
            return NotFound();

        return NoContent();
    }
}