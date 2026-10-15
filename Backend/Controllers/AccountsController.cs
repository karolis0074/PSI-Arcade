using Microsoft.AspNetCore.Authorization;
﻿using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Utils;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private AccountRepository _db = AccountRepository.GetInstance();
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
            Account account = _db.GetAccount(request.Username);

            if (_authService.CheckLogin(account, request.Password))
            {
                return Ok(new { token = _cryptService.GenerateToken(account) }); // returns jwt token
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpPost("changename")]
        public IActionResult ChangeDisplayName(ChangeDisplayNameRequest request)
        {
            var result = _accountService.ChangeDisplayName(GetUsername(), request.NewName);

            return result switch
            {
                AccountResult.Success => Ok(),
                AccountResult.NotFound => NotFound(),
                AccountResult.InvalidInput => BadRequest(),
                _ => StatusCode(500)
            };
        }

        [Authorize]
        [HttpPost("changepass")]
        public IActionResult ChangePassword(ChangePasswordRequest request)
        {
            var username = GetUsername();
            if (!_authService.CheckLogin(username, request.OldPassword))
                return Unauthorized();

            var result = _accountService.ChangePassword(username, request.NewPassword);

            return result switch
            {
                AccountResult.Success => Ok(),
                AccountResult.NotFound => NotFound(),
                AccountResult.InvalidInput => BadRequest(),
                _ => StatusCode(500)
            };
        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult DeleteAccount()
        {
            var result = _db.DeleteAccount(GetUsername());

            return result switch
            {
                AccountResult.Success => Ok(),
                AccountResult.NotFound => NotFound(),
                _ => StatusCode(500)
            };
        }
    }
}
