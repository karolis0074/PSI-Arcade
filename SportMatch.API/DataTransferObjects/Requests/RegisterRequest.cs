using System.ComponentModel.DataAnnotations;

namespace SportMatch.API.DataTransferObjects.Requests;

public record RegisterRequest(
    [Required]
    [StringLength(20, MinimumLength = 3)]
    string DisplayName,

    [Required]
    [StringLength(20, MinimumLength = 3)]
    string Username,

    [Required]
    [StringLength(30, MinimumLength = 8)]
    string Password
);
