using SportMatch.API.Extensions;
using SportMatch.API.Results;

namespace SportMatch.API.Tests.Unit.Extensions;

// unit tests: checks the text that the controller sends back for every result
public class MembershipResultExtensionsTests
{
    [Theory]
    [InlineData(MembershipResult.Success, "Success")]
    [InlineData(MembershipResult.GameNotFound, "Game not found")]
    [InlineData(MembershipResult.GameStarted, "Game has already started")]
    [InlineData(MembershipResult.AlreadyJoined, "User already joined this game")]
    [InlineData(MembershipResult.GameFull, "Game is full")]
    [InlineData(MembershipResult.NotJoined, "User is not in this game")]
    [InlineData(MembershipResult.HostCannotLeave, "Host cannot leave the game")]
    [InlineData(MembershipResult.NotHost, "Only the host can kick players")]
    [InlineData(MembershipResult.CannotKickHost, "Host cannot be kicked")]
    public void ToMessage_ReturnsMessageForResult(MembershipResult result, string expected)
    {
        // act
        var message = result.ToMessage();

        // assert
        Assert.Equal(expected, message);
    }
}
