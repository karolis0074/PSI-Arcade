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

    // GET /sportfields
    // GET /sportfields?city=Vilnius&gameType=Football
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] string? city,
        [FromQuery] string? gameType,
        [FromQuery] string? fieldType)
    {
        return Ok(_sportFieldRepo.Search(city, gameType, fieldType));
    }

    // GET /sportfields/1
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var field = _sportFieldRepo.GetById(id);
        if (field is null)
            return NotFound();
        return Ok(field);
    }
}
