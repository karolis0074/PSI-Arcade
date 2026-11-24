using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Controllers;
using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;
using SportMatch.API.Results;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Unit.Controllers;

// unit tests: GameMembershipController gets a fake service, so only the controller is tested
public class GameMembershipControllerTests
{
    private const int GameId = 1;

    private readonly FakeGameMembershipService _service = new();

    // makes a controller with a logged-in user, so User.Identity.Name works
    private GameMembershipController CreateController(string username)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "TestAuth");

        return new GameMembershipController(_service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            }
        };
    }

    // ---------- Join ----------

    [Fact]
    public void Join_ServiceReturnsSuccess_ReturnsOkWithGame()
    {
        // arrange
        _service.Result = MembershipResult.Success;
        _service.Game = new Game { Id = GameId };
        var controller = CreateController("jonas");

        // act
        var result = controller.Join(GameId);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(GameId, Assert.IsType<Game>(ok.Value).Id);
    }

    [Fact]
    public void Join_LoggedInUser_PassesUsernameAndGameIdToService()
    {
        // arrange
        var controller = CreateController("jonas");

        // act
        controller.Join(GameId);

        // assert
        Assert.Equal("jonas", _service.LastUsername);
        Assert.Equal(GameId, _service.LastGameId);
    }

    [Fact]
    public void Join_ServiceReturnsGameNotFound_ReturnsNotFoundWithMessage()
    {
        // arrange
        _service.Result = MembershipResult.GameNotFound;
        var controller = CreateController("jonas");

        // act
        var result = controller.Join(GameId);

        // assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Game not found", notFound.Value);
    }

    [Fact]
    public void Join_ServiceReturnsGameFull_ReturnsBadRequestWithMessage()
    {
        // arrange
        _service.Result = MembershipResult.GameFull;
        var controller = CreateController("jonas");

        // act
        var result = controller.Join(GameId);

        // assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Game is full", badRequest.Value);
    }

    // ---------- Leave ----------

    [Fact]
    public void Leave_ServiceReturnsSuccess_ReturnsOkWithGame()
    {
        // arrange
        _service.Result = MembershipResult.Success;
        _service.Game = new Game { Id = GameId };
        var controller = CreateController("jonas");

        // act
        var result = controller.Leave(GameId);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(GameId, Assert.IsType<Game>(ok.Value).Id);
    }

    [Fact]
    public void Leave_LoggedInUser_PassesUsernameAndGameIdToService()
    {
        // arrange
        var controller = CreateController("jonas");

        // act
        controller.Leave(GameId);

        // assert
        Assert.Equal("jonas", _service.LastUsername);
        Assert.Equal(GameId, _service.LastGameId);
    }

    [Fact]
    public void Leave_ServiceReturnsGameNotFound_ReturnsNotFoundWithMessage()
    {
        // arrange
        _service.Result = MembershipResult.GameNotFound;
        var controller = CreateController("jonas");

        // act
        var result = controller.Leave(GameId);

        // assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Game not found", notFound.Value);
    }

    [Fact]
    public void Leave_ServiceReturnsHostCannotLeave_ReturnsBadRequestWithMessage()
    {
        // arrange
        _service.Result = MembershipResult.HostCannotLeave;
        var controller = CreateController("host");

        // act
        var result = controller.Leave(GameId);

        // assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Host cannot leave the game", badRequest.Value);
    }

    // ---------- GetSpots ----------

    [Fact]
    public void GetSpots_ServiceReturnsSpots_ReturnsOkWithSpots()
    {
        // arrange
        var spots = new GameSpots();
        spots.Joined = 3;
        spots.Max = 10;
        spots.Free = 7;
        _service.Spots = spots;
        var controller = CreateController("jonas");

        // act
        var result = controller.GetSpots(GameId);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(spots, Assert.IsType<GameSpots>(ok.Value));
        Assert.Equal(GameId, _service.LastGameId);
    }

    [Fact]
    public void GetSpots_ServiceReturnsNull_ReturnsNotFoundWithMessage()
    {
        // arrange
        _service.Spots = null;
        var controller = CreateController("jonas");

        // act
        var result = controller.GetSpots(GameId);

        // assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Game not found", notFound.Value);
    }
}
