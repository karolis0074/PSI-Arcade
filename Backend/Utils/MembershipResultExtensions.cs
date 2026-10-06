using Backend.Data;

namespace SportMatch.API.Utils
{
    public static class MembershipResultExtensions
    {
        // text that is sent back when join or leave fails
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

            return "Success";
        }
    }
}
