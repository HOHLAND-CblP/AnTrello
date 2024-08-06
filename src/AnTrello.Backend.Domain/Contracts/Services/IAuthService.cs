using AnTrello.Backend.Domain.Contracts.Dtos.Auth.Login;
using AnTrello.Backend.Domain.Contracts.Dtos.Auth.Register;
using AnTrello.Backend.Domain.Contracts.Dtos.AuthService.GetNewTokens;
using AnTrello.Backend.Domain.Contracts.Dtos.AuthService.Login;
using AnTrello.Backend.Domain.Contracts.Dtos.UserService.Create;
using Microsoft.AspNetCore.Http;

namespace AnTrello.Backend.Domain.Contracts.Services;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request, CancellationToken token);
    Task<RegisterResponse> Register(CreateUserRequest request, CancellationToken token);
    Task<GetNewTokensResponse> GetNewTokens(string refreshToken, CancellationToken token);
}