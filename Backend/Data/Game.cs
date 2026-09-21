namespace Backend.Data;

public class Game
{
    public int Id { get; set; }
    public int FootballFieldId { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public int MaxPlayers { get; set; }
    public GameStatus Status { get; set; } = GameStatus.Open;
    public List<string> JoinedUsernames { get; set; } = new();
}