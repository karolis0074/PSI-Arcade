using SportMatch.API.Data;

namespace SportMatch.API.Models;

public class Game
{
    // game is almost full when 80% of spots are taken
    private const double AlmostFullRatio = 0.8;

    public int Id { get; set; }
    public int FootballFieldId { get; set; }
    public SportField? FootballField { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public int MaxPlayers { get; set; }
    public GameStatus Status { get; set; } = GameStatus.Open;
    public List<string> JoinedUsernames { get; set; } = new();

    public void UpdateStatus()
    {
        int joined = JoinedUsernames.Count;

        if (joined >= MaxPlayers)
            Status = GameStatus.Full;
        else if ((double)joined / MaxPlayers >= AlmostFullRatio)
            Status = GameStatus.AlmostFull;
        else
            Status = GameStatus.Open;
    }
}