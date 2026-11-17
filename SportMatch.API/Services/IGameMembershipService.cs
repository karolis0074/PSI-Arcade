using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Services
{
    public interface IGameMembershipService
    {
        public Game? GetGame(int gameId);
        public int GetFreeSpots(Game game);
        public GameSpots? GetSpots(int gameId);
        public MembershipResult Join(int gameId, string username);
        public MembershipResult Leave(int gameId, string username);
    }
}
