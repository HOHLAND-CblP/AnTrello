using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Repositories;

public interface IPomodoroSessionRepository 
{
    Task<PomodoroSession> Get(long id, CancellationToken token);
    Task<PomodoroSession> GetTodaySession(DateTime today, long userId, CancellationToken token);
    Task<long> Create(PomodoroSession session, CancellationToken token);
    Task Update(PomodoroSession session, CancellationToken token);
    Task Delete(long id, CancellationToken token);
}