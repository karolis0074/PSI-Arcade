using SportMatch.API.Models;

namespace SportMatch.API.Results;

public record CreateGameResult(Game? Game, string? Error);

public enum DeleteGameResult
{
    Success,
    NotFound,
    NotHost
}
