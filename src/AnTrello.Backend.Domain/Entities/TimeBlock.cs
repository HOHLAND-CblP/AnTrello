namespace AnTrello.Backend.Domain.Entities;

public class TimeBlock
{
    public long Id { get; set; }
    public string Name { get; set; }      
    public string Color { get; set; }     
    public int Duration { get; set; }  
    public int Order { get; set; }   
    public long UserId { get; set; }   
    public DateTime CreatedAt { get; set; }     
    public DateTime? UpdatedAt { get; set; }
}