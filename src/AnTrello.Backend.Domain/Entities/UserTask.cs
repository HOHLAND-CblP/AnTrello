using AnTrello.Backend.Domain.Entities.Jwt;

namespace AnTrello.Backend.Domain.Entities;

public class UserTask
{
    public long Id { get; set; }          
    public string Name { get; set; }        
    public bool IsCompleted { get; set; }
    public long UserId { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }     
    public DateTime? UpdatedAt { get; set; }
}   