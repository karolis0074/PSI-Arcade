using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly AuthService _authService;

    public AccountsController(AccountService accountService, AuthService authService)
    {
        _accountService = accountService;
        _authService = authService;
    }

    private string GetUsername() => User.Identity!.Name!;

    [HttpGet("getname")]
    public IActionResult GetDisplayName([FromQuery] string username)
    {
        var displayName = _accountService.GetDisplayName(username);
        if (displayName is null)
            return NotFound();

        return Ok(new { DisplayName = displayName });
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
        var token = _authService.Login(request.Username, request.Password);
        if (token is null)
            return Unauthorized();

        return Ok(new { token }); // returns jwt token
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
        var result = _accountService.DeleteAccount(GetUsername());

        return result switch
        {
            AccountResult.Success => Ok(),
            AccountResult.NotFound => NotFound(),
            _ => StatusCode(500)
        };
    }
}
