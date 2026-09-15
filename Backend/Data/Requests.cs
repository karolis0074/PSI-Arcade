using System.ComponentModel.DataAnnotations;

namespace Backend
{
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

    public record LoginRequest(
        [Required] string Username,
        [Required] string Password
    );

    public record ChangePasswordRequest(
        [Required] string Username,
        [Required] string OldPassword,

        [Required]
        [StringLength(20, MinimumLength = 3)] 
        string NewPassword
    );
}
