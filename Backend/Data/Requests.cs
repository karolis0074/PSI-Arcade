using System.ComponentModel.DataAnnotations;

namespace Backend
{
    public record RegisterRequest(
        [Required] string DisplayName,
        [Required] string Username,
        [Required] string Password
    );

    public record LoginRequest(
        [Required] string Username,
        [Required] string Password
    );

    public record ChangePasswordRequest(
        [Required] string Username,
        [Required] string OldPassword,
        [Required] string NewPassword
    );
}
