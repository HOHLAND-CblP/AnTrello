namespace AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.UpdateOrder;

public class UpdateOrderRequest
{
    public long[] Ids { get; init; }
    public long UserId { get; set; }
}