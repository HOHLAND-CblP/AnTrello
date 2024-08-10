using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Dtos.AuthService.Login;

public class LoginResponse
{
    public User User { get; init; }
    public string AccessToken { get; init; }
    public string RefreshToken { get; set; }
}