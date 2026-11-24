using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportMatch.API.Controllers;
using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Unit.Controllers;

// Unit tests: GameRecommendationsController gets a FAKE IGameRecommendationService,
// the scoring itself is tested in GameRecommendationServiceTests.
public class GameRecommendationsControllerTests
{
    private readonly FakeGameRecommendationService _service = new();

    // makes a controller with a logged-in user, so User.Identity.Name works
    private GameRecommendationsController CreateController(string username)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "TestAuth");

        return new GameRecommendationsController(_service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            }
        };
    }

    [Fact]
    public void GetRecommended_WithLocation_ReturnsOkAndPassesUserAndLocation()
    {
        _service.Result.Add(new RecommendedGame(new Game { Id = 1 }, 1.5, 0.8));
        var controller = CreateController("jonas");

        var result = controller.GetRecommended(54.68, 25.28);

        var ok = Assert.IsType<OkObjectResult>(result);
        var games = Assert.IsAssignableFrom<IReadOnlyList<RecommendedGame>>(ok.Value);
        Assert.Equal(1, Assert.Single(games).Game.Id);
        Assert.Equal("jonas", _service.LastUsername);
        Assert.Equal(54.68, _service.LastLatitude);
        Assert.Equal(25.28, _service.LastLongitude);
    }

    [Fact]
    public void GetRecommended_WithoutLocation_ReturnsOkAndPassesNoLocation()
    {
        var controller = CreateController("jonas");

        var result = controller.GetRecommended(null, null);

        Assert.IsType<OkObjectResult>(result);
        Assert.True(_service.WasCalled);
        Assert.Null(_service.LastLatitude);
        Assert.Null(_service.LastLongitude);
    }

    [Theory]
    [InlineData(54.68, null)]
    [InlineData(null, 25.28)]
    public void GetRecommended_OnlyOneCoordinate_ReturnsBadRequest(double? lat, double? lng)
    {
        var controller = CreateController("jonas");

        var result = controller.GetRecommended(lat, lng);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.False(_service.WasCalled);
    }

    [Theory]
    [InlineData(91.0, 25.28)]
    [InlineData(-91.0, 25.28)]
    [InlineData(54.68, 181.0)]
    [InlineData(54.68, -181.0)]
    public void GetRecommended_LocationOutOfRange_ReturnsBadRequest(double lat, double lng)
    {
        var controller = CreateController("jonas");

        var result = controller.GetRecommended(lat, lng);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.False(_service.WasCalled);
    }
}
