using Backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("sportfields")]
public class SportFieldsController : ControllerBase
{
    private readonly SportFieldStore _store;

    public SportFieldsController(SportFieldStore store)
    {
        _store = store;
    }

    // GET /sportfields/getall
    [HttpGet("getall")]
    public IActionResult GetAll()
    {
        return Ok(_store.GetAll());
    }

    // GET /sportfields/get?id=1
    [HttpGet("get")]
    public IActionResult GetById([FromQuery] int id)
    {
        var field = _store.GetById(id);
        if (field is null)
            return NotFound();
        return Ok(field);
    }

    // GET /sportfields/search?city=Vilnius&gameType=Football
    [HttpGet("search")]
    public IActionResult Search(
        [FromQuery] string? city,
        [FromQuery] string? gameType,
        [FromQuery] string? fieldType)
    {
        return Ok(_store.Search(city, gameType, fieldType));
    }
}