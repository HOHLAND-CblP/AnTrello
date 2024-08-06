using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Dtos.UserService.Create;

public class CreateUserResponse
{
    public User User { get; init; }
}