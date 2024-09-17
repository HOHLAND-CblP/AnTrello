using AnTrello.Backend.Domain.Contracts.Dtos.UserService.Update;
using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Repositories;

public interface IUserRepository 
{
    Task<long> CreateUser(User user, CancellationToken token);
    Task<User> GetById(long id, CancellationToken token);
    Task<User> GetByEmail(string email, CancellationToken token);
    Task<User> UpdateUser(UpdateUserRequest user, CancellationToken token);

}