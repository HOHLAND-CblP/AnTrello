using AnTrello.Backend.Domain.Entities;
using AnTrello.Backend.Domain.Contracts.Dtos.TaskService.CreateTask;
using AnTrello.Backend.Domain.Contracts.Dtos.TaskService.DeleteTask;
using AnTrello.Backend.Domain.Contracts.Dtos.TaskService.UpdateTask;

namespace AnTrello.Backend.Domain.Contracts.Services;


public interface ITaskService
{
    Task<UserTask> Create (CreateTaskRequest request, CancellationToken token);
    Task<UserTask> GetTask(long id, CancellationToken token);
    Task<List<UserTask>> GetAllUsersTasks(long userId, CancellationToken token);
    Task<List<UserTask>> GetAllUsersCompletedTasks(long userId, CancellationToken token);
    Task<UserTask> Update(UpdateTaskRequest request, CancellationToken token);
    Task Delete(DeleteTaskRequest request, CancellationToken token);
}