using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Services;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.DataTransferObjects.Requests;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GamesController : ControllerBase
{
    private string GetUsername() => User.Identity!.Name!;
    private readonly GameRepository _gameRepository;
    private readonly AppDbContext _dbContext;
    private readonly GameManagerService _gameManagerService;

    public GamesController(GameRepository gameRepository, AppDbContext dbContext, GameManagerService gameManagerService)
    {
        _gameRepository = gameRepository;
        _dbContext = dbContext;
        _gameManagerService = gameManagerService;
    }

    // GET /games
    [HttpGet]
    public async Task<IActionResult> GetAll() 
    {
        return Ok(await _gameRepository.GetAllAsync());
    }

    // GET /games/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var game = await _gameRepository.GetByIdAsync(id);

        if (game is null)
            return NotFound();

        return Ok(game);
    }

    // POST /games
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateGameRequest request)
    {
        // Konvertuojame DTO i Game modeli
        var game = new Game
        {
            FootballFieldId = request.FootballFieldId,
            StartTime = request.StartTime,
            MaxPlayers = request.MaxPlayers,
            CreatedByUsername = GetUsername()
        };

        if (game.MaxPlayers <= 0)
            return BadRequest("MaxPlayers must be greater than 0");

        if (game.StartTime <= DateTime.UtcNow)
            return BadRequest("StartTime must be in the future");

        var field = await _dbContext.SportFields.FirstOrDefaultAsync(f => f.Id == game.FootballFieldId);
        if (field is null)
            return BadRequest("Football field not found");

        var created = await _gameRepository.AddAsync(game);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // DELETE /games/1
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var game = await _gameRepository.GetByIdAsync(id);

        if (game is null)
            return NotFound();

        if (!_gameManagerService.IsHost(GetUsername(), game))
            return Unauthorized();

        // Paliekame sinchronini trynima, nes DeleteAsync metodas nebuvo sukurtas 
        var success = _gameRepository.Delete(id);

        if (!success)
            return NotFound();

        return NoContent();
    }
}