using AnTrello.Backend.Domain.Entities.Jwt;

namespace AnTrello.Backend.Domain.Contracts.Repositories;

public interface ITokenRepository 
{
    Task CreateRefreshToken(JwtRefreshToken refreshToken, CancellationToken token);
    //Task<JwtRefreshToken> GetToken(long id, CancellationToken token);
    Task<bool> IsRefreshTokenActivated(string refreshToken, CancellationToken token);

    Task<JwtRefreshToken> GetRefreshToken(string refreshToken, CancellationToken token);

    Task ActivateToken(string refreshToken, CancellationToken token);

    Task ActivateAllTokens(long userId, CancellationToken token);
    //System.Threading.Tasks.Task DeleteToken(long id, CancellationToken token);
}