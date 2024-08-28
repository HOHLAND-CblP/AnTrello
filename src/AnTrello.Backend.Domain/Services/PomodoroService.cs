using AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.DeleteSession;
using AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.UpdateRound;
using AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.UpdateSession;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Services;

public class PomodoroService : IPomodoroService
{
    private readonly IPomodoroSessionRepository _sessionRepository;
    private readonly IPomodoroRoundRepository _roundRepository;
    
    private readonly IUserService _userService;

    public PomodoroService(
        IPomodoroSessionRepository sessionRepository, 
        IPomodoroRoundRepository roundRepository, 
        IUserService userService)
    {
        _sessionRepository = sessionRepository;
        _roundRepository = roundRepository;
        _userService = userService;
    }
    
    
    public async Task<PomodoroSession> GetTodaySession(long userId, CancellationToken token)
    {
        var today = DateTime.Today;
        
        return await _sessionRepository.GetTodaySession(today, userId, token);
    }
    

    public async Task<PomodoroSession> Create(long userId, CancellationToken token)
    {
        var todaySession = await GetTodaySession(userId, token);
         
        if (todaySession != null) return todaySession;

        var user = await _userService.GetById(userId, token);
        
        if (user == null) throw new ArgumentException("User not found");

        var session = new PomodoroSession
        {
            TotalSeconds = 0,
            IsCompleted = false,
            UserId = userId,
            CreatedAt = DateTime.Now
        };
        
        session.Id = await _sessionRepository.Create(session, token);

        List<PomodoroRound> rounds = new List<PomodoroRound>();

        for (int i = 0; i < user.IntervalsCount; i++)
        {
            rounds.Add(new PomodoroRound
            {
                TotalSeconds = 0,
                IsCompleted = false,
                PomodoroSessionId = session.Id,
                CreatedAt = DateTime.Now,
            });
        }
        
        var ids = await _roundRepository.CreateMany(rounds, token);
        
        for (int i = 0; i < user.IntervalsCount; i++)
        {
            rounds[i].Id = ids[i];
        }
        
        session.Rounds = rounds;
        
        return session;
    }
    

    public async Task UpdateSession(UpdateSessionRequest request, CancellationToken token)
    {
        var session = await _sessionRepository.Get(request.Id, token);
        
        if(session == null || session.UserId != request.UserId) throw new ArgumentException("Session not found");

        session.IsCompleted = request.IsComplete; 
        
        await _sessionRepository.Update(session, token);
    }

    
    public async Task UpdateRound(UpdateRoundRequest request, CancellationToken token)
    {
        var round = await _roundRepository.Get(request.Id, token);
        if (round == null) throw new ArgumentException("Round not found");
        
        var session = await _sessionRepository.Get(round.PomodoroSessionId, token);
        if (session.UserId != request.UserId) throw new ArgumentException("Round not found");

        round.IsCompleted = request.IsCompeted;
        round.TotalSeconds = request.TotalSeconds;
        
        await _roundRepository.Update(round, token);
    }
    

    public async Task DeleteSession(DeleteSessionRequest request, CancellationToken token)
    {   
        var session = await _sessionRepository.Get(request.Id, token);
        if (session == null || session.UserId != request.UserId) throw new ArgumentException("Session not found");
        
        await _sessionRepository.Delete(request.Id, token);
    }
}