using Backend.Data;
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
            var result = _accountService.Register(
                request.DisplayName,
                request.Username,
                request.Password
            );

            return result switch
            {
                RegisterResult.Success => Ok(),
                RegisterResult.UsernameTaken => Conflict(),
                RegisterResult.InvalidInput => BadRequest(),
                _ => StatusCode(500)
            };
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

            var result = _accountService.ChangeDisplayName(request.Username, request.NewName);

            return result switch
            {
                AccountResult.Success => Ok(),
                AccountResult.NotFound => NotFound(),
                AccountResult.InvalidInput => BadRequest(),
                _ => StatusCode(500)
            };
        }

        [HttpPost("changepass")]
        public IActionResult ChangePassword(ChangePasswordRequest request)
        {
            if (!_authService.CheckLogin(request.Username, request.OldPassword))
                return Unauthorized();

             var result = _accountService.ChangePassword(request.Username, request.NewPassword);

            return result switch
            {
                AccountResult.Success => Ok(),
                AccountResult.NotFound => NotFound(),
                AccountResult.InvalidInput => BadRequest(),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("delete")]
        public IActionResult DeleteAccount(LoginRequest request)
        {
            if (!_authService.CheckLogin(request.Username, request.Password))
                return Unauthorized();

            var result = _db.DeleteAccount(request.Username);

            return result switch
            {
                AccountResult.Success => Ok(),
                AccountResult.NotFound => NotFound(),
                _ => StatusCode(500)
            };
        }
    }
}
