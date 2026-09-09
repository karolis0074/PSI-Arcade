using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public record CreateAccountRequest(
        [Required] string Username,
        [Required] string Password
    );

    public record LoginRequest(
        [Required] string Username,
        [Required] string Password
    );
}
