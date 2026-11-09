using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Repositories;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("sportfields")]
public class SportFieldsController : ControllerBase
{
    private readonly SportFieldRepository _sportFieldRepository;

    public SportFieldsController(SportFieldRepository sportFieldRepository)
    {
        _sportFieldRepository = sportFieldRepository;
    }

    // GET /sportfields
    // GET /sportfields?city=Vilnius&gameType=Football
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] string? city,
        [FromQuery] string? gameType,
        [FromQuery] string? fieldType)
    {
        return Ok(_sportFieldRepository.Search(city, gameType, fieldType));
    }

    // GET /sportfields/1
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var field = _sportFieldRepository.GetById(id);
        if (field is null)
            return NotFound();
        return Ok(field);
    }
}
