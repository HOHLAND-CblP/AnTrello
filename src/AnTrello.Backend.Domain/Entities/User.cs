using System.ComponentModel;

namespace AnTrello.Backend.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string Email { get; init; }
    public string? Name { get; init; }
    [PasswordPropertyText]
    public string Password { get; init; }
    public int BreakInterval { get; set; }
    public int IntervalsCount { get; set; }
    public int WorkInterval { get; set; }   
    public DateTime CreatedAt { get; init; }     
    public DateTime? UpdatedAt { get; set; }
}