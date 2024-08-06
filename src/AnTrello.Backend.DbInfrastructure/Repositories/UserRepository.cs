using AnTrello.Backend.DbInfrastructure.Settings;
using Dapper;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Entities;
using Microsoft.Extensions.Options;

namespace AnTrello.Backend.DbInfrastructure.Repositories;

public class UserRepository: MainRepository,  IUserRepository
{
    public UserRepository(IOptions<DbSettings> settings) : base(settings.Value.PostgresConnectionString){}


    public async Task<long> CreateUser(User user, CancellationToken token)
    {
        string sql =
            """
            INSERT INTO users(
            email, name, password, break_interval, intervals_count, work_interval, created_at)
            VALUES (@Email, @Name, @Password, @BreakInterval, @IntervalsCount, @WorkInterval, @CreatedAt)
            returning id;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<long>(
            new CommandDefinition(
                sql,
                new
                {
                    Email = user.Email,
                    Name = user.Name,
                    Password = user.Password,
                    BreakInterval = user.BreakInterval,
                    IntervalsCount = user.IntervalsCount,
                    WorkInterval = user.WorkInterval,
                    CreatedAt = user.CreatedAt
                },
                cancellationToken: token
            ))).FirstOrDefault();
    }

    public async Task<User> GetById(long id, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM users
            WHERE id=@Id;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<User>(
            new CommandDefinition(
                sql,
                new
                {
                    Id = id
                },
                cancellationToken: token
            ))).FirstOrDefault();
    }

    public async Task<User> GetByEmail(string email, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM users
            WHERE email=@Email;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<User>(
            new CommandDefinition(
                sql,
                new
                {
                    Email = email
                },
                cancellationToken: token
            ))).FirstOrDefault();
    }

    
}