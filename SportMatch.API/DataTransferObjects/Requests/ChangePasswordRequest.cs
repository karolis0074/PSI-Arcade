using System.ComponentModel.DataAnnotations;

namespace SportMatch.API.DataTransferObjects.Requests;

public record ChangePasswordRequest(
    [Required] string OldPassword,

    [Required]
    [StringLength(30, MinimumLength = 8)]
    string NewPassword
);
