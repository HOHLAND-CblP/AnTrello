namespace AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.DeleteSession;

public class DeleteSessionRequest
{
    public long Id { get; set; }
    public long UserId { get; set; }
}