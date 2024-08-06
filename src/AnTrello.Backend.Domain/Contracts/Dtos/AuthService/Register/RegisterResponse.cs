using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Dtos.Auth.Register;

public class RegisterResponse
{
    public User User { get; init; }
    public string AccessToken { get; init; }
    public string RefreshToken { get; set; }
}