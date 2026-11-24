using SportMatch.API.Models;

namespace SportMatch.API.DataTransferObjects.Responses;

// a game with its distance from the user (null when unknown) and its recommendation score (0-1)
public record RecommendedGame(Game Game, double? DistanceKm, double Score);
