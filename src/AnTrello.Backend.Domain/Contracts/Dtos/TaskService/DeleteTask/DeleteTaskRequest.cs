namespace AnTrello.Backend.Domain.Contracts.Dtos.TaskService.DeleteTask;

public class DeleteTaskRequest
{
    public long Id { get; init; }
    public long UserId { get; init; }
}