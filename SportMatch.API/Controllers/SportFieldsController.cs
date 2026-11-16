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

    // GET /sportfields/getall
    [HttpGet("getall")]
    public async Task<IActionResult> GetAll() 
    {
        return Ok(await _sportFieldRepository.GetAllAsync());
    }

    // GET /sportfields/get?id=1
    [HttpGet("get")]
    public async Task<IActionResult> GetById([FromQuery] int id)
    {
        var field = await _sportFieldRepository.GetByIdAsync(id);
        
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
        return Ok(_sportFieldRepository.Search(city, gameType, fieldType));
    }
}