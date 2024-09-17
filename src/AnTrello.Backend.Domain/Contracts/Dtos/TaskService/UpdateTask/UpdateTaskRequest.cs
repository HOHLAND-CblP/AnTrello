using System.Text.Json.Serialization;
using System.Text.Json.Serialization;
using AnTrello.Backend.Domain.Entities;
using AnTrello.Backend.Domain.Entities.Jwt;

namespace AnTrello.Backend.Domain.Contracts.Dtos.TaskService.UpdateTask;

public class UpdateTaskRequest
{
    public long Id { get; set; }
    public long UserId { get; set; }
    
    public string? Name { get; set; }        
    public bool? IsCompleted { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]  
    public TaskPriority? Priority { get; set; }
    public DateTime? CreatedAt { get; set; }        
}