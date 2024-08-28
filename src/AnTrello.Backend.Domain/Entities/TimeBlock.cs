namespace AnTrello.Backend.Domain.Entities;

public class TimeBlock
{
    public long Id { get; set; }
    public string Name { get; init; }      
    public string Color { get; init; }     
    public int Duration { get; init; }  
    public int Order { get; init; }   
    public long UserId { get; init; }   
    public DateTime CreatedAt { get; init; }     
    public DateTime? UpdatedAt { get; init; }
}