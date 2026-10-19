using System.ComponentModel.DataAnnotations;

namespace SportMatch.API.DataTransferObjects.Requests;

public record CreateGameRequest(
    [Range(1, int.MaxValue)] int FootballFieldId,
    [Required] DateTime StartTime,
    [Range(1, 100)] int MaxPlayers
);