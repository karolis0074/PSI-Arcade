using SportMatch.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GamesController : ControllerBase
{
    private string GetUsername() => User.Identity!.Name!;
    private readonly GameRepository _store;
    private readonly AppDbContext _db;
    private readonly GameManagerService _gameManager;

    public GamesController(GameRepository store, AppDbContext db, GameManagerService gameManager)
    {
        _store = store;
        _db = db;
        _gameManager = gameManager;
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
    public IActionResult Create(Game game)
    {
        // when tracking game host use GetUsername() 

        // game must have at least 1 player slot.
        if (game.MaxPlayers <= 0)
            return BadRequest("MaxPlayers must be greater than 0");

        // users cannot create games in the past.
        if (game.StartTime <= DateTime.UtcNow)
            return BadRequest("StartTime must be in the future");

        // Patikrinam, ar aikštelė egzistuoja
        var field = _db.SportFields.FirstOrDefault(f => f.Id == game.FootballFieldId);
        if (field is null)
            return BadRequest("Football field not found");

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
            return Unauthorized();

        var success = _store.Delete(id);

        if (!success)
            return NotFound();

        return NoContent();
    }
}