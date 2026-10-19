namespace SportMatch.API.DataTransferObjects.Responses;

public struct GameStatistics
{
    public int TotalGames { get; set; }
    public double AveragePlayers { get; set; }
    public int? MostPopularFieldId { get; set; }
}
