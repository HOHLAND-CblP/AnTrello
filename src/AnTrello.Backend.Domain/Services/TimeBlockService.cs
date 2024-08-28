using System.Transactions;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Create;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Delete;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Update;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.UpdateOrder;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Services;

public class TimeBlockService : ITimeBlockService
{
    private readonly ITimeBlockRepository _repository;

    public TimeBlockService(ITimeBlockRepository repository)
    {
        _repository = repository;
    }
    
    
    public async Task<List<TimeBlock>> GetAll(long userId, CancellationToken token)
    {
        var timeBlocks = await _repository.GetAll(userId, token);

        return timeBlocks;
    }

    public async Task<TimeBlock> Create(CreateTimeBlockRequest request, CancellationToken token)
    {
        var timeBlock = new TimeBlock
        {
            Name = request.Name,
            Color = request.Color,
            Duration = request.Duration,
            Order = request.Order,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };
        
        timeBlock.Id = await _repository.Create(timeBlock, token);

        return timeBlock;
    }

    public async Task<TimeBlock> Update(UpdateTimeBlockRequest request, CancellationToken token)
    {
        var timeBlock = await _repository.Get(request.Id, token);

        if (timeBlock == null || timeBlock.UserId != request.UserId)
        {
            return null;
        }

        timeBlock = new TimeBlock
        {
            Id = timeBlock.Id,
            UserId = timeBlock.UserId,
            
            Name = request.Name ?? timeBlock.Name,
            Color = request.Color ?? timeBlock.Color,
            Duration = request.Duration ?? timeBlock.Duration,
            Order = request.Order ?? timeBlock.Order,
            
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.Update(timeBlock, token);

        return timeBlock;
    }

    public async Task Delete(DeleteTimeBlockRequest request, CancellationToken token)
    {
        var timeBlock = await _repository.Get(request.Id, token);

        if (timeBlock != null && timeBlock.UserId != request.UserId)
        {
            await _repository.Delete(request.Id, token);
        }
    }

    public async Task UpdateOrder(UpdateOrderRequest request, CancellationToken token)
    {
        await _repository.UpdateOrder(request.Ids ,token);
    }
}