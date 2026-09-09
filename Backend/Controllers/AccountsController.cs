using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AccountsController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private string GenerateToken(Account account)
        {
            var claims = new[] { 
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()) 
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int GetCallerId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateAccountRequest request)
        {
            bool usernameTaken = await _context.Accounts.AnyAsync(a => a.Username == request.Username);
            if (usernameTaken)
                return Conflict($"Username '{request.Username}' is already taken.");

            var account = new Account(request.Username, request.Password);
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(account);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == request.Username);

            if (account is null || !account.VerifyPassword(request.Password))
                return Unauthorized("Invalid username or password.");

            return Ok(new { token = GenerateToken(account), account } );
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != GetCallerId()) return Forbid();

            var account = await _context.Accounts.FindAsync(id);
            if (account is null) return NotFound();

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
