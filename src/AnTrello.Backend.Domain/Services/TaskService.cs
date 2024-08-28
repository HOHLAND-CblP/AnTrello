using AnTrello.Backend.Domain.Contracts.Dtos.TaskService.CreateTask;
using AnTrello.Backend.Domain.Contracts.Dtos.TaskService.DeleteTask;
using AnTrello.Backend.Domain.Contracts.Dtos.TaskService.UpdateTask;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    
    public async Task<UserTask> Create(CreateTaskRequest request, CancellationToken token)
    {
        UserTask task = new UserTask
        {
            Name = request.Name,
            IsCompleted = request.IsCompleted,
            UserId = request.UserId,
            Priority = request.Priority,
            CreatedAt = request.CreatedAt
        };
        
        var taskId = await _repository.Create(task, token);

        task.Id = taskId;

        return task;
    }

    public async Task<UserTask> GetTask(long id, CancellationToken token)
    {
        var task = await _repository.Get(id, token);

        return task;
    }

    public async Task<List<UserTask>> GetAllUsersTasks(long userId, CancellationToken token)
    {
        var tasks = await _repository.GetAllUsersTasks(userId, token);

        return tasks;
    }

    public async Task<List<UserTask>> GetAllUsersCompletedTasks(long userId, CancellationToken token)
    {
        var tasks = await _repository.GetAllCompletedTasks(userId, token);

        return tasks;
    }

    public async Task<UserTask> Update(UpdateTaskRequest request, CancellationToken token)
    {
        var task = await _repository.Get(request.Id, token);

        if (task == null || task.UserId != request.UserId)
            return null;

        task = new UserTask
        {
            Name = request.Name ?? task.Name,
            IsCompleted = request.IsCompleted ?? task.IsCompleted,
            Priority = request.Priority ?? task.Priority,
            CreatedAt = request.CreatedAt ?? task.CreatedAt,
        };
        
        
        await _repository.Update(task, token);

        return task;
    }

    public async Task Delete(DeleteTaskRequest request, CancellationToken token)
    {
        var task = await _repository.Get(request.Id, token);
        
        if (task == null || task.UserId != request.UserId)
            throw new UnauthorizedAccessException("You do not have permission to delete this task.");
        
        await _repository.Delete(request.Id, token);
    }
}