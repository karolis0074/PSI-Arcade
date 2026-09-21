using Microsoft.AspNetCore.Mvc;
using Backend.Utils;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private Database _db = Database.GetInstance();
        private TokenService _tokens = TokenService.GetInstance();
        private AccountService _accountService;
        private AuthService _authService;

        public AccountsController()
        {
            _accountService = new AccountService(_db, _tokens);
            _authService = new AuthService(_db, _tokens);
        }

        private string? GetBearerToken()
        {
            string? header = Request.Headers.Authorization.ToString();
            if (String.IsNullOrWhiteSpace(header) || !header.StartsWith("Bearer "))
                return null;

            return header["Bearer ".Length..];
        }

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
            if (!_authService.CheckLogin(request.Username, request.Password))
                return Unauthorized();

            var account = _db.GetAccount(request.Username)!;
            var token = _authService.IssueToken(account);
            return Ok(new { token });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _authService.RevokeToken(GetBearerToken());
            return Ok();
        }

        [HttpPost("changename")]
        public IActionResult ChangeDisplayName(ChangeDisplayNameRequest request)
        {
            var account = _authService.AuthenticateToken(GetBearerToken());
            if (account == null)
                return Unauthorized();

            int returnCode = _accountService.ChangeDisplayName(account, request.NewName);

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
            var account = _authService.AuthenticateToken(GetBearerToken());
            if (account == null || !_authService.CheckLogin(account, request.OldPassword))
                return Unauthorized();

            int returnCode = _accountService.ChangePassword(account, request.NewPassword);

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
        public IActionResult DeleteAccount()
        {
            var account = _authService.AuthenticateToken(GetBearerToken());
            if (account == null)
                return Unauthorized();

            int returnCode = _db.DeleteAccount(account.Username);

            switch (returnCode)
            {
                case 0:
                    _authService.RevokeToken(GetBearerToken());
                    return Ok();

                case 1:
                    return NotFound();

                default:
                    return StatusCode(500);
            }
        }
    }
}
