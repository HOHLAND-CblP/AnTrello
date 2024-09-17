namespace AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.UpdateRound;

public class UpdateRoundRequest
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public int TotalSeconds { get; init; }
    public bool IsCompleted { get; init; }
}