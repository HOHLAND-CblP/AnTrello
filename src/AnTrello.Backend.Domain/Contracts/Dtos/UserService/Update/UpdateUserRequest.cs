using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Dtos.UserService.Update;

public class UpdateUserRequest
{
    public User User { get; init; }
}