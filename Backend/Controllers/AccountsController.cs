using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        Database DB = Database.GetInstance();

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            int returnCode = DB.AddAccount(
                new Account(
                    request.DisplayName,
                    request.Username,
                    request.Password
                )
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
            Account account = DB.GetAccount(request.Username);
            if (account == null || !account.CheckPassword(request.Password))
                return Unauthorized();

            // later need to implement tokens/cookies so user can actually
            // log in and be authorized to do other actions
            return Ok();
        }

        [HttpPost("changepass")]
        public IActionResult ChangePassword(ChangePasswordRequest request)
        {
            Account account = DB.GetAccount(request.Username);
            if (account == null)
                return Unauthorized();

            int returnCode = account.ChangePassword(request.OldPassword, request.NewPassword);
            switch (returnCode)
            {
                case 0:
                    return Ok();

                case 1:
                    return Unauthorized(); // incorrect old password

                case 2:
                    return BadRequest(); // new password is invalid

                default:
                    return StatusCode(500);
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteAccount(LoginRequest request)
        {
            Account account = DB.GetAccount(request.Username);
            if (account == null || account.CheckPassword(request.Password))
                return Unauthorized();

            if (DB.DeleteAccount(request.Username) == 0)
                return Ok();

            return StatusCode(500);
        }
    }
}
