using System.ComponentModel.DataAnnotations;

namespace SportMatch.API.DataTransferObjects.Requests;

public record ChangeDisplayNameRequest(
    [Required]
    [StringLength(20, MinimumLength = 3)]
    string NewName
);
