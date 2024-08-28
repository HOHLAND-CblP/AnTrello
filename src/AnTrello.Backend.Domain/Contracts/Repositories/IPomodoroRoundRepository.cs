using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Repositories;

public interface IPomodoroRoundRepository 
{
    Task<PomodoroRound> Get(long requestId, CancellationToken token);
    Task<List<long>> CreateMany(List<PomodoroRound> rounds, CancellationToken token);
    Task Update(PomodoroRound round, CancellationToken token);
}