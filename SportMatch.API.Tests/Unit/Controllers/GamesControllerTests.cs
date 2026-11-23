using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Controllers;
using SportMatch.API.Data;
using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Unit.Controllers;

// GamesController reads and writes games through GameRepository (a real one on an
// in-memory SQLite database) and only asks IGameManagerService (a FAKE) who the host is.
// Every result is checked through the HTTP responses the controller returns.
public class GamesControllerTests
{
    private const int ExistingFieldId = 1; // TestDatabase always adds field 1

    private readonly AppDbContext _db = TestDatabase.Create();
    private readonly FakeGameManagerService _service = new();

    // makes a controller with a logged-in user, so User.Identity.Name works
    private GamesController CreateController(string username)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "TestAuth");

        return new GamesController(new GameRepository(_db), _db, _service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            }
        };
    }

    // puts a game straight into the test database
    private Game AddGame(string host)
    {
        var game = new Game
        {
            FootballFieldId = ExistingFieldId,
            CreatedByUsername = host,
            StartTime = DateTime.UtcNow.AddDays(1),
            MaxPlayers = 10
        };
        _db.Games.Add(game);
        _db.SaveChanges();
        return game;
    }

    // the controller rejects games in the past, so the start time is always tomorrow
    private static CreateGameRequest ValidRequest() => new(ExistingFieldId, DateTime.UtcNow.AddDays(1), 4);

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
    public async Task Create_ValidRequest_ReturnsCreatedGameHostedByLoggedInUser()
    {
        var controller = CreateController("petras");
        var request = ValidRequest();

        var result = await controller.Create(request);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var game = Assert.IsType<Game>(created.Value);
        Assert.Equal(nameof(GamesController.GetById), created.ActionName);
        Assert.Equal(game.Id, created.RouteValues!["id"]);
        Assert.Equal("petras", game.CreatedByUsername);
        Assert.Equal(request.FootballFieldId, game.FootballFieldId);
        Assert.Single(_db.Games);
    }

    [Fact]
    public async Task Create_FieldDoesNotExist_ReturnsBadRequestWithMessage()
    {
        var controller = CreateController("petras");
        var request = ValidRequest() with { FootballFieldId = 999 };

        var result = await controller.Create(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Football field not found", badRequest.Value);
        Assert.Empty(_db.Games);
    }

    [Fact]
    public async Task Create_StartTimeInPast_ReturnsBadRequest()
    {
        var controller = CreateController("petras");
        var request = ValidRequest() with { StartTime = DateTime.UtcNow.AddDays(-1) };

        var result = await controller.Create(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("StartTime must be in the future", badRequest.Value);
    }

    [Fact]
    public async Task Create_MaxPlayersZero_ReturnsBadRequest()
    {
        var controller = CreateController("petras");
        var request = ValidRequest() with { MaxPlayers = 0 };

        var result = await controller.Create(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("MaxPlayers must be greater than 0", badRequest.Value);
    }

    // ---------- DELETE ----------

    [Fact]
    public async Task Delete_HostDeletesOwnGame_ReturnsNoContent()
    {
        var game = AddGame(host: "petras");
        var controller = CreateController("petras");

        var result = await controller.Delete(game.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_db.Games);
    }

    [Fact]
    public async Task Delete_NotHost_ReturnsUnauthorized()
    {
        var game = AddGame(host: "petras");
        var controller = CreateController("jonas");

        var result = await controller.Delete(game.Id);

        Assert.IsType<UnauthorizedResult>(result);
        Assert.Single(_db.Games);
    }

    [Fact]
    public async Task Delete_GameDoesNotExist_ReturnsNotFound()
    {
        var controller = CreateController("petras");

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
