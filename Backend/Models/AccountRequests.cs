using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public record CreateAccountRequest(
        [property: Required] string DisplayName,
        [property: Required] string Username,
        [property: Required] string Password
    );

    public record LoginRequest(
        [property: Required] string Username,
        [property: Required] string Password
    );
}
