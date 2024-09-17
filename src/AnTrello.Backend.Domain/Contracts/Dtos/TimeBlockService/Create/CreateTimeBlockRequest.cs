namespace AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Create;

public class CreateTimeBlockRequest
{
    public long UserId { get; set; }
    public string Name { get; init; }
    public string? Color { get; init; }
    public int Duration { get; init; }
    public int Order { get; init; }
}