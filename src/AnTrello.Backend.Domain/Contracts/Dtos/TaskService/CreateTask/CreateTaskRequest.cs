using System.Text.Json.Serialization;
using AnTrello.Backend.Domain.Entities.Jwt;

namespace AnTrello.Backend.Domain.Contracts.Dtos.TaskService.CreateTask;

public class CreateTaskRequest
{
    public string Name { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime CreatedAt { get; init; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]  
    public TaskPriority Priority { get; init; }
    public long UserId { get; set; }
}