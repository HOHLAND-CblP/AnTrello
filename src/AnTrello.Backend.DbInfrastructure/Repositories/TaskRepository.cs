using AnTrello.Backend.DbInfrastructure.Settings;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace AnTrello.Backend.DbInfrastructure.Repositories;

public class TaskRepository : BasePgRepository, ITaskRepository
{
    public TaskRepository(IOptions<DbSettings> settings) : base(settings.Value.PostgresConnectionString)
    {
    }

    
    public async Task<long> Create(UserTask userTask, CancellationToken token)
    {
        string sql =
            """
            INSERT INTO tasks(
                name, is_completed, user_id, priority, created_at, updated_at)
            VALUES (@Name, @IsCompleted, @UserId, @Priority, @CreatedAt, @UpdatedAt)
            returning id;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<long>(
            new CommandDefinition(
                sql,
                new
                {
                    Name = userTask.Name,
                    IsCompleted = userTask.IsCompleted,
                    UserId = userTask.UserId,
                    Priority = userTask.Priority,
                    CreatedAt = userTask.CreatedAt,
                    UpdatedAt = userTask.UpdatedAt
                },
                cancellationToken: token
            ))).FirstOrDefault();   
    }
    
    

    public async Task<UserTask> Get(long id, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM tasks
            WHERE id = @Id;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<UserTask>(
            new CommandDefinition(
                sql,
                new
                {
                    Id = id
                },
                cancellationToken: token
            ))).FirstOrDefault();  
    }

    
    
    public async Task<List<UserTask>> GetAllUsersTasks(long userId, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM tasks
            WHERE user_id = @UserId;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<Domain.Entities.UserTask>(
            new CommandDefinition(
                sql,
                new
                {
                    UserId = userId
                },
                cancellationToken: token
            ))).ToList();
    }

    public async Task<List<UserTask>> GetAllCompletedTasks(long userId, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM tasks
            WHERE user_id = @UserId AND is_completed;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<UserTask>(
            new CommandDefinition(
                sql,
                new
                {
                    UserId = userId
                },
                cancellationToken: token
            ))).ToList();
    }

    
    public async Task<List<UserTask>> GetTasksBeforeDate(long userId, DateTime date, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM tasks
            WHERE user_id = @UserId AND created_at >= current_date AND created_at <= @Date;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<UserTask>(
            new CommandDefinition(
                sql,
                new
                {
                    UserId = userId,
                    Date = date
                },
                cancellationToken: token
            ))).ToList();
    }
    
    

    public async Task Update(UserTask userTask, CancellationToken token)
    {
        string sql =
            """
            UPDATE tasks
            SET name = @Name, 
                is_completed = @IsCompleted,
                priority = @Priority,
                created_at = @CreatedAt,
                updated_at = @UpdatedAt
            WHERE id = @Id;
            """;
        
        await using var connection = await GetConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Id = userTask.Id,
                    Name = userTask.Name,
                    IsCompleted = userTask.IsCompleted,
                    Priority = userTask.Priority,
                    CreatedAt = userTask.CreatedAt,
                    UpdatedAt = userTask.UpdatedAt
                },
                cancellationToken: token
            ));
    }

    
    
    public async Task Delete(long id, CancellationToken token)
    {
        string sql =
            """
            DELETE FROM tasks
            WHERE id = @Id;
            """;
        
        await using var connection = await GetConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Id = id
                },
                cancellationToken: token
            ));
    }
}