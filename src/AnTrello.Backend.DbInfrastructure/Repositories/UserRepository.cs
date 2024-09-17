using AnTrello.Backend.DbInfrastructure.Settings;
using AnTrello.Backend.Domain.Contracts.Dtos.UserService.Update;
using Dapper;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Entities;
using Microsoft.Extensions.Options;

namespace AnTrello.Backend.DbInfrastructure.Repositories;

public class UserRepository: BasePgRepository,  IUserRepository
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

    public async Task<User> UpdateUser(UpdateUserRequest request, CancellationToken token)
    {
        string sql =
            """
            UPDATE users
            """;
        
        var conditions = new List<string>();
        var @params = new DynamicParameters();

        if (request.Email != null)
        {
            conditions.Add("email = @Email");
            @params.Add("@Email", request.Email);
        }
        if (request.Name != null)
        {
            conditions.Add("name = @Name");
            @params.Add("@Name", request.Name);
        }

        if (request.Password != null)
        {
            conditions.Add("password = @Password");
            @params.Add("@Password", request.Password);
        }
        if (request.BreakInterval != null)
        {
            conditions.Add("break_interval = @BreakInterval");
            @params.Add("@BreakInterval", request.BreakInterval);
        }
        if (request.IntervalsCount != null)
        {
            conditions.Add("intervals_count = @IntervalsCount");
            @params.Add("@IntervalsCount", request.IntervalsCount);
        }
        if (request.WorkInterval != null)
        {
            conditions.Add("work_interval = @WorkInterval");
            @params.Add("@WorkInterval", request.WorkInterval);
        }

        if (conditions.Count == 0)
            return null;
        
        conditions.Add("updated_at = @UpdatedAt");
        @params.Add("@UpdatedAt", DateTime.UtcNow);
        
        sql += $"\nSET\n\t{string.Join(",\n\t", conditions)}";
        
        conditions = new List<string>();
        
        conditions.Add("WHERE id = @Id");
        @params.Add("@Id", request.Id);

        conditions.Add("returning id;");
        sql += $"\n{string.Join("\n", conditions)}";
        
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<User>(
            new CommandDefinition(
                sql,
                @params,
                cancellationToken: token
            ))).FirstOrDefault();
    }
}