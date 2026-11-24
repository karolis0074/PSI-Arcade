using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Exceptions;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IAuthService _authService;

    public AccountsController(IAccountService accountService, IAuthService authService)
    {
        _accountService = accountService;
        _authService = authService;
    }

    private string GetUsername() => User.Identity!.Name!;

    // POST /accounts
    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var result = await _accountService.RegisterAsync(request.Username, request.Password);

            return result switch
            {
                RegisterResult.Success => Ok(),
                RegisterResult.UsernameTaken => Conflict(),
                RegisterResult.InvalidInput => BadRequest(),
                _ => StatusCode(500)
            };
        }
        catch (InvalidAccountException ex)
        {
            return BadRequest(ex.Message); // tells the client what was wrong
        }
    }

    // POST /accounts/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _authService.LoginAsync(request.Username, request.Password);
        if (token is null)
            return Unauthorized();

        return Ok(new { token }); // returns jwt token
    }

    // PUT /accounts/me/password
    [Authorize]
    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var username = GetUsername();
        if (!await _authService.CheckLoginAsync(username, request.OldPassword))
            return Unauthorized();

        var result = await _accountService.ChangePasswordAsync(username, request.NewPassword);

        return result switch
        {
            AccountResult.Success => Ok(),
            AccountResult.NotFound => NotFound(),
            AccountResult.InvalidInput => BadRequest(),
            _ => StatusCode(500)
        };
    }

    // DELETE /accounts/me
    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAccount()
    {
        var result = await _accountService.DeleteAccountAsync(GetUsername());

        return result switch
        {
            AccountResult.Success => Ok(),
            AccountResult.NotFound => NotFound(),
            _ => StatusCode(500)
        };
    }
}
