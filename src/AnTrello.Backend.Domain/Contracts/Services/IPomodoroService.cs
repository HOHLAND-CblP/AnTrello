using AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.DeleteSession;
using AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.UpdateRound;
using AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.UpdateSession;
using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Services;

public interface IPomodoroService
{
    Task<PomodoroSession> GetTodaySession(long userId, CancellationToken token);
    Task<PomodoroSession> Create(long userId, CancellationToken token);
    Task UpdateSession(UpdateSessionRequest request, CancellationToken token);
    Task UpdateRound(UpdateRoundRequest request, CancellationToken token);
    Task DeleteSession(DeleteSessionRequest request, CancellationToken token);
}