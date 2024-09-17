using Microsoft.Extensions.Options;
using AnTrello.Backend.DbInfrastructure.Settings;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Entities;
using Dapper;


namespace AnTrello.Backend.DbInfrastructure.Repositories;

public class PomodoroRoundRepository: BasePgRepository, IPomodoroRoundRepository
{
    public PomodoroRoundRepository(IOptions<DbSettings> settings) : base(settings.Value.PostgresConnectionString) { }


    public async Task<PomodoroRound> Get(long id, CancellationToken token)
    {
        string sql =
            """
            SELECT *
            FROM pomodoro_rounds 
            WHERE id = @Id
            """;
        
        await using var connection = await GetConnection();

        return (await connection.QueryAsync<PomodoroRound>(
            new CommandDefinition(
                sql,
                new
                {
                    Id = id
                },
                cancellationToken: token
            ))).FirstOrDefault();
    }

    public async Task<List<long>> CreateMany(List<PomodoroRound> rounds, CancellationToken token)
    {
        string sql =
            """
            INSERT INTO pomodoro_rounds(
                total_seconds, is_completed, pomodoro_session_id, created_at)
            SELECT total_seconds, is_completed, pomodoro_session_id, created_at
            FROM UNNEST(@Rounds)
            returning id;
            """;

        
        await using var connection = await GetConnection();

        return (await connection.QueryAsync<long>(
            new CommandDefinition(
                sql,
                new
                {
                    Rounds = rounds,
                },
                cancellationToken: token
            ))).ToList();
    }

    public async Task Update(PomodoroRound round, CancellationToken token)
    {
        string sql =
            """
            UPDATE pomodoro_rounds
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
                    Id = round.Id,
                    TotalSeconds = round.TotalSeconds,
                    IsCompleted = round.IsCompleted,
                    UpdatedAt = round.UpdatedAt,
                },
                cancellationToken: token
            ));
    }
}