using System.ComponentModel.DataAnnotations;

namespace AnTrello.Backend.Domain.Contracts.Dtos.UserService.Create;

public class CreateUserRequest
{
    [EmailAddress(ErrorMessage = "Not valid email")]
    public string Email { get; init; }
    [MaxLength(30, ErrorMessage = "Max name length is 30 characters")]
    public string? Name { get; init; }
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; init; }
}