using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Repositories;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("sportfields")]
public class SportFieldsController : ControllerBase
{
    private readonly ISportFieldRepository _sportFieldRepository;

    public SportFieldsController(ISportFieldRepository sportFieldRepository)
    {
        _sportFieldRepository = sportFieldRepository;
    }

    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] string? city = null,
        [FromQuery] string? gameType = null,
        [FromQuery] string? fieldType = null)
    {
        return Ok(_sportFieldRepository.Search(city, gameType, fieldType));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var field = _sportFieldRepository.GetById(id);
        
        if (field is null)
            return NotFound();
            
        return Ok(field);
    }
}