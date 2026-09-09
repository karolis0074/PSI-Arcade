using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

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

            return Ok(account);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account is null) return NotFound();

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
