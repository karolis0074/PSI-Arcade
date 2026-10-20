using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("games")]
public class GamesController : ControllerBase
{
    private string GetUsername() => User.Identity!.Name!;
    private readonly GameManagerService _gameManager;

    public GamesController(GameManagerService gameManager)
    {
        _gameManager = gameManager;
    }

    // GET /games/getall
    [HttpGet("getall")]
    public IActionResult GetAll()
    {
        return Ok(_gameManager.GetAll());
    }

    // GET /games/get?id=1
    [HttpGet("get")]
    public IActionResult GetById([FromQuery] int id)
    {
        var game = _gameManager.GetById(id);

        if (game is null)
            return NotFound();

        return Ok(game);
    }

    // POST /games/create
    [Authorize]
    [HttpPost("create")]
    public IActionResult Create(CreateGameRequest request)
    {
        var result = _gameManager.Create(GetUsername(), request);

        if (result.Game is null)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Game.Id }, result.Game);
    }

    // DELETE /games/delete?id=1
    [Authorize]
    [HttpDelete("delete")]
    public IActionResult Delete([FromQuery] int id)
    {
        var result = _gameManager.Delete(id, GetUsername());

        return result switch
        {
            DeleteGameResult.Success => NoContent(),
            DeleteGameResult.NotFound => NotFound(),
            DeleteGameResult.NotHost => Forbid(),   // 403
            _ => StatusCode(500)
        };
    }
}
