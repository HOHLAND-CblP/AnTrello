using System.Data.Common;
using Microsoft.Extensions.Options;
using AnTrello.Backend.DbInfrastructure.Settings;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.UpdateOrder;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Entities;
using Dapper;
using Npgsql;

namespace AnTrello.Backend.DbInfrastructure.Repositories;

public class TimeBlockRepository : BasePgRepository, ITimeBlockRepository
{
    public TimeBlockRepository(IOptions<DbSettings> settings) : base(settings.Value.PostgresConnectionString) {}
    
    
    public async Task<List<TimeBlock>> GetAll(long userId, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM time_blocks
            WHERE user_Id = @UserId
            ORDER BY "order" ASC;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<TimeBlock>(
            new CommandDefinition(
                sql,
                new
                {
                    UserId = userId
                },
                cancellationToken: token
            ))).ToList(); 
    }

    
    public async Task<TimeBlock> Get(long id, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM time_blocks
            WHERE id = @Id;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<TimeBlock>(
            new CommandDefinition(
                sql,
                new
                {
                    Id = id
                },
                cancellationToken: token
            ))).FirstOrDefault(); 
    }

    
    public async Task<long> Create(TimeBlock timeBlock, CancellationToken token)
    {
        string sql =
            """
            INSERT INTO time_blocks(
                name, color, duration, "order", user_id,created_at)
            VALUES (@Name, @Color, @Duration, @Order, @UserId, @CreatedAt)
            returning id;
            """;
        
        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<long>(
            new CommandDefinition(
                sql,
                new
                {
                    Name = timeBlock.Name,
                    Color = timeBlock.Color,
                    Duration = timeBlock.Duration,
                    Order = timeBlock.Order,
                    UserId = timeBlock.UserId,
                    CreatedAt = timeBlock.CreatedAt
                },
                cancellationToken: token
            ))).FirstOrDefault(); 
    }

    
    public async Task Update(TimeBlock timeBlock, CancellationToken token)
    {
        string sql =
            """
            UPDATE time_blocks
            SET name = @Name, 
                color = @Color,
                duration = @Duration,
                "order" = @Order,
                updated_at = @UpdatedAt
            WHERE id = @Id;
            """;
        
        await using var connection = await GetConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Id = timeBlock.Id,
                    Name = timeBlock.Name,
                    Color = timeBlock.Color,
                    Duration = timeBlock.Duration,
                    Order = timeBlock.Order,
                    UpdatedAt = timeBlock.UpdatedAt,
                },
                cancellationToken: token
            ));
    }
    
    
    public async Task UpdateOrder(long[] ids, CancellationToken token)
    {
        string sql =
            """
            UPDATE time_blocks
            SET "order" = @Order,
                updated_at = @UpdatedAt
            WHERE id = @Id;
            """;

        await using var connection = await GetConnection();

        
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        try
        {
            for (int i = 0; i < ids.Length; i++)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            @Order = i,
                            @UpdatedAt = DateTime.UtcNow,
                            @Id = ids[i]
                        },
                        cancellationToken: token,
                        transaction: transaction
                    ));
            }
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(token);
            throw;
        }
        
        await transaction.CommitAsync(token);
    }

    
    public async Task Delete(long id, CancellationToken token)
    {
        string sql =
            """
            DELETE FROM time_blocks
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