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
    private readonly LiveMetricsService _metrics; 

    public AccountsController(IAccountService accountService, IAuthService authService, LiveMetricsService metrics)
    {
        _accountService = accountService;
        _authService = authService;
        _metrics = metrics;
    }

    private string GetUsername() => User.Identity!.Name!;

    // POST /accounts
    [HttpPost]
    public IActionResult Register(RegisterRequest request)
    {
        try
        {
            var result = _accountService.Register(
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
        catch (InvalidAccountException ex)
        {
            return BadRequest(ex.Message); // tells the client what was wrong
        }
    }

    // POST /accounts/login
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var token = _authService.Login(request.Username, request.Password);
        if (token is null)
            return Unauthorized();

        _metrics.UserConnected(); // Padidiname aktyvių vartotojų skaičių

        return Ok(new { token }); // returns jwt token
    }

    // POST /accounts/logout
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _metrics.UserDisconnected(); //Sumažiname aktyvių vartotojų skaičių
        return Ok(new { message = "Sėkmingai atsijungta" });
    }

    // PUT /accounts/me/password
    [Authorize]
    [HttpPut("me/password")]
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

    // DELETE /accounts/me
    [Authorize]
    [HttpDelete("me")]
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