namespace AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.UpdateSession;

public class UpdateSessionRequest
{ 
    public long Id { get; set; }
    public long UserId { get; set; }
    public bool IsCompleted { get; init; }
}