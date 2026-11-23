using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Controllers;
using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Models;
using SportMatch.API.Results;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Unit.Controllers;

// Unit tests: GamesController gets a FAKE repository and a FAKE IGameManagerService,
// and every result is checked through the HTTP responses the controller returns.
// The business rules themselves (time, field, capacity) are tested in GameManagerServiceTests.
public class GamesControllerTests
{
    private readonly FakeGameRepository _gameRepository = new();
    private readonly FakeGameManagerService _service = new();

    // makes a controller with a logged-in user, so User.Identity.Name works
    private GamesController CreateController(string username)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "TestAuth");

        return new GamesController(_gameRepository, _service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            }
        };
    }

    // fixed date: the fake service does not check time, so the test never depends on the clock
    private static CreateGameRequest ValidRequest() => new(1, new DateTime(2026, 1, 2, 12, 0, 0, DateTimeKind.Utc), 4);

    // puts a game straight into the fake repository
    private Game AddGame(string host) => _gameRepository.Add(new Game { CreatedByUsername = host });

    // ---------- GET ----------

    [Fact]
    public async Task GetAll_TwoGamesExist_ReturnsOkWithBothGames()
    {
        AddGame(host: "host");
        AddGame(host: "host");
        var controller = CreateController("jonas");

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var games = Assert.IsAssignableFrom<IReadOnlyList<Game>>(ok.Value);
        Assert.Equal(2, games.Count);
    }

    [Fact]
    public async Task GetById_GameExists_ReturnsOkWithGame()
    {
        var game = AddGame(host: "host");
        var controller = CreateController("jonas");

        var result = await controller.GetById(game.Id);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(game.Id, Assert.IsType<Game>(ok.Value).Id);
    }

    [Fact]
    public async Task GetById_GameDoesNotExist_ReturnsNotFound()
    {
        const int missingId = 999;
        var controller = CreateController("jonas");

        var result = await controller.GetById(missingId);

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
        _service.CreateError = "MaxPlayers cannot exceed the field capacity (22)";
        var controller = CreateController("petras");

        var result = controller.Create(ValidRequest());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("MaxPlayers cannot exceed the field capacity (22)", badRequest.Value);
    }

    // ---------- DELETE ----------

    [Fact]
    public void Delete_ServiceReturnsSuccess_ReturnsNoContent()
    {
        _service.DeleteResult = DeleteGameResult.Success;
        var controller = CreateController("petras");

        var result = controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Delete_ServiceReturnsNotHost_ReturnsForbid()
    {
        _service.DeleteResult = DeleteGameResult.NotHost;
        var controller = CreateController("jonas");

        var result = controller.Delete(1);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public void Delete_ServiceReturnsNotFound_ReturnsNotFound()
    {
        _service.DeleteResult = DeleteGameResult.NotFound;
        var controller = CreateController("petras");

        var result = controller.Delete(1);

        Assert.IsType<NotFoundResult>(result);
    }
}
