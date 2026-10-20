using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Repositories;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("sportfields")]
public class SportFieldsController : ControllerBase
{
    private readonly SportFieldRepository _sportFieldRepo;

    public SportFieldsController(SportFieldRepository sportFieldRepo)
    {
        _sportFieldRepo = sportFieldRepo;
    }

    // GET /sportfields/getall
    [HttpGet("getall")]
    public IActionResult GetAll() => Ok(_sportFieldRepo.GetAll());

    // GET /sportfields/get?id=1
    [HttpGet("get")]
    public IActionResult GetById([FromQuery] int id)
    {
        var field = _sportFieldRepo.GetById(id);
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
        return Ok(_sportFieldRepo.Search(city, gameType, fieldType));
    }
}
