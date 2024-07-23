namespace AnTrello.Backend.Domain.Entities;

public class Task
{
    public long Id { get; set; }          
    public string Name { get; set; }        
    public bool IsCompleted { get; set; }
    public long UserId { get; set; }
    public string Priority { get; set; }
    public DateTime CreatedAt { get; set; }     
    public DateTime? UpdatedAt { get; set; }
}