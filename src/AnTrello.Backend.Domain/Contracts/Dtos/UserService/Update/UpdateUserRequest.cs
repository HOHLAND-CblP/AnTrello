using System.ComponentModel.DataAnnotations;
using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Dtos.UserService.Update;

public class UpdateUserRequest
{
    public long Id { get; set; }
    public string? Email { get; init; }
    public string? Name { get; init; }
    public string? Password { get; set; } 
    [Range(1, int.MaxValue)]
    public int? BreakInterval { get; set; }
    [Range(1,10)]
    public int? IntervalsCount { get; set; }
    [Range(1, int.MaxValue)]
    public int? WorkInterval { get; set; }   
}