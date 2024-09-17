using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Repositories;

public interface ITaskRepository 
{
    Task<long> Create(Entities.UserTask userTask, CancellationToken token);
    Task<UserTask> Get(long id, CancellationToken token);
    Task<List<UserTask>> GetAllUsersTasks(long userId, CancellationToken token);
    Task<List<UserTask>> GetAllCompletedTasks(long userId, CancellationToken token);
    // Возвращает все таски с сегоднящнего дня до указаной даты включительно
    Task<List<UserTask>> GetTasksBeforeDate(long userId, DateTime date, CancellationToken token); 
    Task Update(UserTask userTask, CancellationToken token);
    Task Delete(long id, CancellationToken token);
}