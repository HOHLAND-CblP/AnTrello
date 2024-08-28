namespace AnTrello.Backend.Domain.Entities;

public class PomodoroRound
{
    public long Id { get; set; }           
    public long TotalSeconds { get; set; }
    public bool IsCompleted { get; set; } 
    public long PomodoroSessionId { get; init; }       
    public DateTime CreatedAt { get; init; }     
    public DateTime? UpdatedAt { get; init; }
}