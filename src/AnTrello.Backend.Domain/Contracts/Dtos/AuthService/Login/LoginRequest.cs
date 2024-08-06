using System.ComponentModel.DataAnnotations;

namespace AnTrello.Backend.Domain.Contracts.Dtos.Auth.Login;

public class LoginRequest
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}