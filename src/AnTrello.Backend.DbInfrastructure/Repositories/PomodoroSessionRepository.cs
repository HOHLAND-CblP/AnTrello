using Microsoft.Extensions.Options;
using AnTrello.Backend.DbInfrastructure.Settings;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Entities;
using Dapper;

namespace AnTrello.Backend.DbInfrastructure.Repositories;

public class PomodoroSessionRepository: BasePgRepository, IPomodoroSessionRepository
{
    public PomodoroSessionRepository(IOptions<DbSettings> settings) : base(settings.Value.PostgresConnectionString)
    {
    }

    public async Task<PomodoroSession> Get(long id, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM pomodoro_sessions
            WHERE id = @Id;

            SELECT *
            FROM pomodoro_rounds 
            WHERE pomodoro_session_id = @Id
            """;
        
        await using var connection = await GetConnection();
        
        await using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Id = id
                },
                cancellationToken: token
            ));

        var session = await multi.ReadFirstOrDefaultAsync<PomodoroSession>();
        if (session == null) return null;
        
        session.Rounds = (await multi.ReadAsync<PomodoroRound>()).ToList();

        return session;
    }

    public async Task<PomodoroSession> GetTodaySession(DateTime today, long userId, CancellationToken token)
    {  
        string sql =
            """
            SELECT *
            FROM pomodoro_sessions
            WHERE user_id = @UserId AND created_at >= @Today
            LIMIT 1;

            SELECT pr.*
            FROM pomodoro_rounds pr, 
                (  
                    SELECT *
                    FROM pomodoro_sessions
                    WHERE user_id = @UserId AND created_at >= @Today
                    LIMIT 1
                ) as ps              
            WHERE pr.pomodoro_session_id = ps.id
            ORDER BY pr.id;
            """;
        
        await using var connection = await GetConnection();
        
        await using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(
                sql,
                new
                {
                    UserId = userId,
                    Today = today
                },
                cancellationToken: token
            ));

        var session = await multi.ReadFirstOrDefaultAsync<PomodoroSession>();
        if (session == null) return null;
        
        session.Rounds = (await multi.ReadAsync<PomodoroRound>()).ToList();

        return session;
    }

    public async Task<long> Create(PomodoroSession session, CancellationToken token)
    {
        string sql =
            """
            INSERT INTO pomodoro_sessions(
                total_seconds, is_completed, user_id, created_at)
            VALUES (@TotalSeconds, @IsCompleted, @UserId, @CreatedAt)
            returning id;
            """;

        await using var connection = await GetConnection();
        
        return (await connection.QueryAsync<long>(
            new CommandDefinition(
                sql,
                new
                {
                    TotalSeconds = session.TotalSeconds,
                    IsCompleted = session.IsCompleted,
                    UserId = session.UserId,
                    CreatedAt = session.CreatedAt
                },
                cancellationToken: token
            ))).FirstOrDefault(); 
    }

    public async Task Update(PomodoroSession session, CancellationToken token)
    {
        string sql =
            """
            UPDATE pomodoro_sessions
            SET total_seconds = @TotalSeconds,
                is_completed = @IsCompleted,
                updated_at = @UpdatedAt
            WHERE id = @Id;
            """;
        
        await using var connection = await GetConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Id = session.Id,
                    TotalSeconds = session.TotalSeconds,
                    IsCompleted = session.IsCompleted,
                    UpdatedAt = session.UpdatedAt,
                },
                cancellationToken: token
            ));
    }

    public async Task Delete(long id, CancellationToken token)
    {
        string sql =
            """
            DELETE FROM pomodoro_sessions
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