namespace Backend.Data;

public class Game
{
    public int Id { get; set; }
    public int FootballFieldId { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime StartTime { get; set; }
    public int MaxPlayers { get; set; }
    public GameStatus Status { get; set; } = GameStatus.Open; // New games always start as Open.
}