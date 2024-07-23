using AnTrello.Backend.Domain.Contracts.Dtos.User.Login;

namespace AnTrello.Backend.Domain.Contracts.Services;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request, CancellationToken token);
}