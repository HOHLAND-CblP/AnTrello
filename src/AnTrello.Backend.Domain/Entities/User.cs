namespace AnTrello.Backend.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string Email { get; set; }
    public string? Name { get; set; }
    public string Password { get; set; }
    public int BreakInterval { get; set; }
    public int IntervalsCount { get; set; }
    public int WorkInterval { get; set; }   
    public DateTime CreatedAt { get; set; }     
    public DateTime? UpdatedAt { get; set; }
}