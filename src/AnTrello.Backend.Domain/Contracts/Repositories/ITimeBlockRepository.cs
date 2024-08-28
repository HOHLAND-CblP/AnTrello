using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.UpdateOrder;
using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Repositories;

public interface ITimeBlockRepository 
{
    Task<List<TimeBlock>> GetAll(long userId, CancellationToken token);
    Task<TimeBlock> Get(long id, CancellationToken token);
    Task<long> Create(TimeBlock timeBlock, CancellationToken token);
    Task Update(TimeBlock timeBlock, CancellationToken token);
    Task UpdateOrder(long[] ids, CancellationToken token);
    Task Delete(long id, CancellationToken token);
}