using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Create;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Delete;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Update;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.UpdateOrder;
using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Services;

public interface ITimeBlockService
{
    Task<List<TimeBlock>> GetAll(long userId, CancellationToken token);
    Task<TimeBlock> Create(CreateTimeBlockRequest request, CancellationToken token);
    Task<TimeBlock> Update(UpdateTimeBlockRequest request, CancellationToken token);
    Task Delete(DeleteTimeBlockRequest request, CancellationToken token);
    Task UpdateOrder(UpdateOrderRequest request, CancellationToken token);
}