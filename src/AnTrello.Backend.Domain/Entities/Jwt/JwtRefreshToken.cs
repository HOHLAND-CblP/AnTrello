namespace AnTrello.Backend.Domain.Entities.Jwt;

public class JwtRefreshToken
{
    public string Token { get; init; }
    public long UserId { get; init; }
}