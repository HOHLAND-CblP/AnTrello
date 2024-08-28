namespace AnTrello.Backend.Domain.Entities;

public class PomodoroSession
{
    public long Id { get; set; }           
    public long TotalSeconds { get; init; }
    public bool IsCompleted { get; set; }
    public List<PomodoroRound> Rounds { get; set; }
    public long UserId { get; init; }       
    public DateTime CreatedAt { get; init; }     
    public DateTime? UpdatedAt { get; init; }
}