using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Services;
using Microsoft.EntityFrameworkCore;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GamesController : ControllerBase
{
    private string GetUsername() => User.Identity!.Name!;
    private readonly GameRepository _store;
    private readonly AppDbContext _dbContext;
    private readonly GameManagerService _gameManager;

    public GamesController(GameRepository store, AppDbContext dbContext, GameManagerService gameManager)
    {
        _store = store;
        _dbContext = dbContext;
        _gameManager = gameManager;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _store.GetAllAsync());
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetById([FromQuery] int id)
    {
        var game = await _store.GetByIdAsync(id);

        if (game is null)
            return NotFound();

        return Ok(game);
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create(Game game)
    {
        if (game.MaxPlayers <= 0)
            return BadRequest("MaxPlayers must be greater than 0");

        if (game.StartTime <= DateTime.UtcNow)
            return BadRequest("StartTime must be in the future");

        var field = await _dbContext.SportFields.FirstOrDefaultAsync(f => f.Id == game.FootballFieldId);
        if (field is null)
            return BadRequest("Football field not found");

        var created = await _store.AddAsync(game);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var game = await _store.GetByIdAsync(id);

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