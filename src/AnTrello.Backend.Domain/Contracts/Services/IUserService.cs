using AnTrello.Backend.Domain.Contracts.Dtos.UserService.Create;
using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Services;

public interface IUserService
{
    Task<CreateUserResponse> Create(CreateUserRequest request, CancellationToken token);
    Task<User> GetById(long id, CancellationToken token);
    Task<User> GetByEmail(string email, CancellationToken token);
    Task<bool> VerifyUser(string email, string password, CancellationToken token);
}