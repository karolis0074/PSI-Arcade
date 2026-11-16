using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Controllers;
using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Models;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Unit.Controllers;

// Unit tests: GamesController gets a FAKE IGameManagerService, not the real one,
// and every result is checked through the HTTP responses the controller returns.
public class GamesControllerTests
{
    private readonly FakeGameManagerService _service = new();

    // makes a controller with a logged-in user, so User.Identity.Name works
    private GamesController CreateController(string username)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "TestAuth");

        return new GamesController(_service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            }
        };
    }

    private static CreateGameRequest ValidRequest() => new(1, DateTime.UtcNow.AddDays(1), 4);

    // ---------- GET ----------

    [Fact]
    public void GetAll_TwoGamesExist_ReturnsOkWithBothGames()
    {
        _service.AddGame(host: "host");
        _service.AddGame(host: "host");
        var controller = CreateController("jonas");

        var result = controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var games = Assert.IsAssignableFrom<IReadOnlyList<Game>>(ok.Value);
        Assert.Equal(2, games.Count);
    }

    [Fact]
    public void GetById_GameExists_ReturnsOkWithGame()
    {
        var game = _service.AddGame(host: "host");
        var controller = CreateController("jonas");

        var result = controller.GetById(game.Id);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(game.Id, Assert.IsType<Game>(ok.Value).Id);
    }

    [Fact]
    public void GetById_GameDoesNotExist_ReturnsNotFound()
    {
        var controller = CreateController("jonas");

        var result = controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    // ---------- POST ----------

    [Fact]
    public void Create_ValidRequest_ReturnsCreatedGameHostedByLoggedInUser()
    {
        var controller = CreateController("petras");
        var request = ValidRequest();

        var result = controller.Create(request);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var game = Assert.IsType<Game>(created.Value);
        Assert.Equal(nameof(GamesController.GetById), created.ActionName);
        Assert.Equal(game.Id, created.RouteValues!["id"]);
        Assert.Equal("petras", game.CreatedByUsername);
        Assert.Equal(request.FootballFieldId, game.FootballFieldId);
    }

    [Fact]
    public void Create_ServiceReturnsError_ReturnsBadRequestWithMessage()
    {
        _service.CreateError = "Sport field not found";
        var controller = CreateController("petras");

        var result = controller.Create(ValidRequest());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Sport field not found", badRequest.Value);
    }

    // ---------- DELETE ----------

    [Fact]
    public void Delete_UserIsHost_ReturnsNoContentAndGameIsGone()
    {
        var game = _service.AddGame(host: "petras");
        var controller = CreateController("petras");

        var result = controller.Delete(game.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.IsType<NotFoundResult>(controller.GetById(game.Id));
    }

    [Fact]
    public void Delete_UserIsNotHost_ReturnsForbidAndGameStays()
    {
        var game = _service.AddGame(host: "petras");
        var controller = CreateController("jonas");

        var result = controller.Delete(game.Id);

        Assert.IsType<ForbidResult>(result);
        Assert.IsType<OkObjectResult>(controller.GetById(game.Id));
    }

    [Fact]
    public void Delete_GameDoesNotExist_ReturnsNotFound()
    {
        var controller = CreateController("petras");

        var result = controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
