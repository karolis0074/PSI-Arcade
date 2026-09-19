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

        [HttpGet("getname")]
        public IActionResult GetDisplayName(UserDataRequest request)
        {
            var account = _db.GetAccount(request.Username);
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
            if (_authService.CheckLogin(request.Username, request.Password))
            {
                // later need to implement tokens/cookies so user can actually
                // log in and be authorized to do other actions
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost("changename")]
        public IActionResult ChangeDisplayName(ChangeDisplayNameRequest request)
        {
            if (!_authService.CheckLogin(request.Username, request.Password))
                return Unauthorized();

            int returnCode = _accountService.ChangeDisplayName(request.Username, request.NewName);

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

        [HttpPost("changepass")]
        public IActionResult ChangePassword(ChangePasswordRequest request)
        {
            if (!_authService.CheckLogin(request.Username, request.OldPassword))
                return Unauthorized();

            int returnCode = _accountService.ChangePassword(request.Username, request.NewPassword);

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

        [HttpDelete("delete")]
        public IActionResult DeleteAccount(LoginRequest request)
        {
            if (!_authService.CheckLogin(request.Username, request.Password))
                return Unauthorized();

            int returnCode = _db.DeleteAccount(request.Username);
            
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
