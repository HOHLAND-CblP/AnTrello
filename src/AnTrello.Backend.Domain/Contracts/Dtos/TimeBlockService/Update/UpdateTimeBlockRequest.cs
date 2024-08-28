namespace AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Update;

public class UpdateTimeBlockRequest
{
    public long Id { get; set; }
    public long UserId { get; set; }
    
    public string? Name { get; set; }
    public string? Color { get; set; }
    public int? Duration { get; set; }
    public int? Order { get; set; }
}