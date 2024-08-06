using AnTrello.Backend.DbInfrastructure.Settings;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Entities.Jwt;
using Dapper;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace AnTrello.Backend.DbInfrastructure.Repositories;

public class TokenRepository(IOptions<DbSettings> settings)
    : MainRepository(settings.Value.PostgresConnectionString), ITokenRepository
{
    public async Task CreateRefreshToken(JwtRefreshToken refreshToken, CancellationToken token)
    {
        string sql =
            """
            INSERT INTO jwt_refresh_tokens(token, user_id)
            VALUES (@Token, @UserId);
            """;
        
        await using var connection = await GetConnection();
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    UserId = refreshToken.UserId,
                    Token = refreshToken.Token
                },
                cancellationToken: token
            ));
    }

    public async Task<bool?> IsRefreshTokenActivated(string refreshToken, CancellationToken token)
    {
        string sql =
            """
            SELECT is_activated
            FROM jwt_refresh_tokens
            WHERE token=@Token;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<bool?>(
            new CommandDefinition(
                sql,
                new
                {
                    Token = refreshToken
                },
                cancellationToken: token
            ))).FirstOrDefault();
    }

    public async Task<JwtRefreshToken> GetRefreshToken(string refreshToken, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM jwt_refresh_tokens
            WHERE token=@Token;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<JwtRefreshToken>(
            new CommandDefinition(
                sql,
                new
                {
                    Token = refreshToken
                },
                cancellationToken: token
            ))).FirstOrDefault();
    }

    public async Task ActivateToken(string refreshToken, CancellationToken token)
    {
        string sql =
            """
            UPDATE jwt_refresh_tokens
                SET is_activated=true
                WHERE token = @RefreshToken
            """;
        
        await using var connection = await GetConnection();
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    RefreshToken = refreshToken
                },
                cancellationToken: token
            ));
        
    }
}