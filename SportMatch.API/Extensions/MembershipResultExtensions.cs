using SportMatch.API.Results;

namespace SportMatch.API.Extensions;

public static class MembershipResultExtensions
{
    // text that is sent back when join, leave or kick fails
    public static string ToMessage(this MembershipResult result)
    {
        if (result == MembershipResult.GameNotFound)
            return "Game not found";

        if (result == MembershipResult.GameStarted)
            return "Game has already started";

        if (result == MembershipResult.AlreadyJoined)
            return "User already joined this game";

        if (result == MembershipResult.GameFull)
            return "Game is full";

        if (result == MembershipResult.NotJoined)
            return "User is not in this game";

        if (result == MembershipResult.HostCannotLeave)
            return "Host cannot leave the game";

        if (result == MembershipResult.NotHost)
            return "Only the host can kick players";

        if (result == MembershipResult.CannotKickHost)
            return "Host cannot be kicked";

        return "Success";
    }
}
