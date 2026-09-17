namespace Backend.Data;

// Represents a user's participation in a game.
// Record because it's a data holder (compared by value, not reference).
public record GamePlayer(int GameId, int UserId, DateTime JoinedAt);