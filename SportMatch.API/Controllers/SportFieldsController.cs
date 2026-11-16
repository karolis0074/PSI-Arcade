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
    public async Task<IActionResult> GetAll(
        [FromQuery] string? city = null,
        [FromQuery] string? gameType = null,
        [FromQuery] string? fieldType = null)
    {
        return Ok(_sportFieldRepository.Search(city, gameType, fieldType));
    }

    // GET /sportfields/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var field = await _sportFieldRepository.GetByIdAsync(id);
        
        if (field is null)
            return NotFound();
            
        return Ok(field);
    }
}