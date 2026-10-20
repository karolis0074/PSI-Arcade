using System.ComponentModel.DataAnnotations;

namespace SportMatch.API.DataTransferObjects.Requests;

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);
