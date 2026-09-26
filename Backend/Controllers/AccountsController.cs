using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Utils;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private Database _db = Database.GetInstance();
        private AccountService _accountService = new AccountService();
        private AuthService _authService = new AuthService();
        private CryptService _cryptService = new CryptService();


        private string GetUsername() => User.Identity!.Name!;
             
        [HttpGet("getname")]
        public IActionResult GetDisplayName([FromQuery] string username)
        {
            var account = _db.GetAccount(username);
            if (account == null)
                return NotFound();

            return Ok(new { account.DisplayName });
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            int returnCode = _accountService.Register(
                request.DisplayName,
                request.Username,
                request.Password
            );

            switch (returnCode)
            {
                case 0:
                    return Ok();

                case 1:
                    return Conflict(); // user already exists

                case 2:
                    return BadRequest(); // account could not be created

                default:
                    return StatusCode(500);
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            Account account = _db.GetAccount(request.Username);

            if (_authService.CheckLogin(account, request.Password))
            {
                return Ok(_cryptService.GenerateToken(account)); // returns jwt token
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpPost("changename")]
        public IActionResult ChangeDisplayName(ChangeDisplayNameRequest request)
        {
            int returnCode = _accountService.ChangeDisplayName(GetUsername(), request.NewName);

            switch (returnCode)
            {
                case 0:
                    return Ok();

                case 1:
                    return NotFound();

                case 2:
                    return BadRequest();

                default:
                    return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPost("changepass")]
        public IActionResult ChangePassword(ChangePasswordRequest request)
        {
            var username = GetUsername();
            if (!_authService.CheckLogin(username, request.OldPassword))
                return Unauthorized();

            int returnCode = _accountService.ChangePassword(username, request.NewPassword);

            switch (returnCode)
            {
                case 0:
                    return Ok();

                case 2:
                    return BadRequest();

                default:
                    return StatusCode(500);
            }
        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult DeleteAccount()
        {
            int returnCode = _db.DeleteAccount(GetUsername());
            
            switch (returnCode)
            {
                case 0:
                    return Ok();

                case 1:
                    return NotFound();

                default:
                    return StatusCode(500);
            }
        }
    }
}
