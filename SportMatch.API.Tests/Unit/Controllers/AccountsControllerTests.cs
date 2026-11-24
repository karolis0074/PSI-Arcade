using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Controllers;
using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Results;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Unit.Controllers;

public class AccountsControllerTests
{
    private readonly FakeAccountService _accountService = new();
    private readonly FakeAuthService _authService = new();

    private AccountsController CreateController(string username)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "TestAuth");

        return new AccountsController(_accountService, _authService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            }
        };
    }

    private static RegisterRequest ValidRegisterRequest() => new("petras", "password123");

    private static LoginRequest ValidLoginRequest() => new("petras", "password123");

    // register

    [Fact]
    public async Task Register_ServiceReturnsSuccess_ReturnsOk()
    {
        _accountService.RegisterResult = RegisterResult.Success;
        var controller = CreateController("jonas");

        var result = await controller.Register(ValidRegisterRequest());

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Register_ServiceReturnsUsernameTaken_ReturnsConflict()
    {
        _accountService.RegisterResult = RegisterResult.UsernameTaken;
        var controller = CreateController("jonas");

        var result = await controller.Register(ValidRegisterRequest());

        Assert.IsType<ConflictResult>(result);
    }

    // login

    [Fact]
    public async Task Login_ServiceReturnsToken_ReturnsOkWithToken()
    {
        _authService.Token = "jwt-token";
        var controller = CreateController("jonas");

        var result = await controller.Login(ValidLoginRequest());

        var ok = Assert.IsType<OkObjectResult>(result);
        // anonymous object, so read by name
        var token = ok.Value!.GetType().GetProperty("token")!.GetValue(ok.Value);
        Assert.Equal("jwt-token", token);
    }

    [Fact]
    public async Task Login_ServiceReturnsNoToken_ReturnsUnauthorized()
    {
        _authService.Token = null;
        var controller = CreateController("jonas");

        var result = await controller.Login(ValidLoginRequest());

        Assert.IsType<UnauthorizedResult>(result);
    }

    // change password

    [Fact]
    public async Task ChangePassword_OldPasswordCorrect_ReturnsOkForLoggedInUser()
    {
        _authService.CheckLoginResult = true;
        _accountService.ChangePasswordResult = AccountResult.Success;
        var controller = CreateController("petras");

        var result = await controller.ChangePassword(new ChangePasswordRequest("password123", "newpassword123"));

        Assert.IsType<OkResult>(result);
        Assert.Equal("petras", _accountService.LastUsername);
    }

    // delete

    [Fact]
    public async Task DeleteAccount_ServiceReturnsSuccess_ReturnsOkForLoggedInUser()
    {
        _accountService.DeleteResult = AccountResult.Success;
        var controller = CreateController("petras");

        var result = await controller.DeleteAccount();

        Assert.IsType<OkResult>(result);
        Assert.Equal("petras", _accountService.LastUsername);
    }
}
