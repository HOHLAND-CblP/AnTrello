using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AnTrello.Backend.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string Email { get; init; }
    public string? Name { get; init; }
    [JsonIgnore]
    public string Password { get; init; } 
    [Range(1, int.MaxValue)]
    public int BreakInterval { get; set; }
    [Range(1,10)]
    public int IntervalsCount { get; set; }
    [Range(1, int.MaxValue)]
    public int WorkInterval { get; set; }   
    public DateTime CreatedAt { get; init; }     
    public DateTime? UpdatedAt { get; set; }
}