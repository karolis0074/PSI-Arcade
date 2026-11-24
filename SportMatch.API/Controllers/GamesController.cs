using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Repositories;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GamesController : ControllerBase
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameManagerService _gameManagerService;

    public GamesController(IGameRepository gameRepository, IGameManagerService gameManagerService)
    {
        _gameRepository = gameRepository;
        _gameManagerService = gameManagerService;
    }

    private string GetUsername() => User.Identity!.Name!;

    // GET /games
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _gameRepository.GetAllAsync());
    }

    // GET /games/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var game = await _gameRepository.GetByIdAsync(id);

        if (game is null)
            return NotFound();

        return Ok(game);
    }

    // POST /games
    // all the rules (start time, field exists, field capacity) live in GameManagerService
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateGameRequest request)
    {
        var result = await _gameManagerService.CreateAsync(GetUsername(), request);

        if (result.Game is null)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Game.Id }, result.Game);
    }

    // DELETE /games/1
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _gameManagerService.DeleteAsync(id, GetUsername());

        return result switch
        {
            DeleteGameResult.Success => NoContent(),
            DeleteGameResult.NotFound => NotFound(),
            DeleteGameResult.NotHost => Forbid(),   // 403
            _ => StatusCode(500)
        };
    }
}
