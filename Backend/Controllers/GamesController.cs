using Backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("games")]
public class GamesController : ControllerBase
{
    private readonly GameStore _store = new();

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_store.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var game = _store.GetById(id);

        if (game is null)
            return NotFound();

        return Ok(game);
    }

    [HttpPost]
    public IActionResult Create(Game game)
    {
        //game must have at least 1 player slot.
        if (game.MaxPlayers <= 0)
            return BadRequest("MaxPlayers must be greater than 0");

        //users cannot create games in the past.
        if (game.StartTime <= DateTime.UtcNow)
            return BadRequest("StartTime must be in the future");

        var created = _store.Add(game);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var success = _store.Delete(id);

        if (!success)
            return NotFound();

        return NoContent();
    }
}