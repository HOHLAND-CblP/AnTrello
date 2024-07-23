using System.ComponentModel.DataAnnotations;

namespace AnTrello.Backend.Domain.Contracts.Dtos.User.Login;

public class LoginRequest
{
    [EmailAddress]
    public string Email { get; set; }
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; }
}