using System.Text.Json.Serialization;

namespace SportMatch.API.Models;

public class Account : BaseEntity
{
    public string Username { get; init; } = string.Empty;

    [JsonIgnore] public string PasswordHash { get; set; } = string.Empty;
}
