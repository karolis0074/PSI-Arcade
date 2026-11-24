using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;
using SportMatch.API.Results;
namespace SportMatch.API.Services;
public interface IGameMembershipService
{
    Task<Game?> GetGameAsync(int gameId);
    Task<GameSpots?> GetSpotsAsync(int gameId);
    Task<MembershipResult> JoinAsync(int gameId, string username);
    Task<MembershipResult> LeaveAsync(int gameId, string username);
}