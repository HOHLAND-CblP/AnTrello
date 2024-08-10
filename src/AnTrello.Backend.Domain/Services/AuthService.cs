using System.IdentityModel.Tokens.Jwt;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using AnTrello.Backend.Domain.Contracts.Dtos.Auth.Login;
using AnTrello.Backend.Domain.Contracts.Dtos.Auth.Register;
using AnTrello.Backend.Domain.Contracts.Dtos.AuthService.GetNewTokens;
using AnTrello.Backend.Domain.Contracts.Dtos.AuthService.Login;
using AnTrello.Backend.Domain.Contracts.Dtos.UserService.Create;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;
using AnTrello.Backend.Domain.Entities.Jwt;
using AnTrello.Backend.Domain.Settings;
using JwtConstants = System.IdentityModel.Tokens.Jwt.JwtConstants;
using Task = AnTrello.Backend.Domain.Entities.Task;

namespace AnTrello.Backend.Domain.Services;

internal class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly ITokenRepository _tokenRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserService userService, ITokenRepository tokenRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userService = userService;
        _tokenRepository = tokenRepository;
        _jwtSettings = jwtSettings.Value;
    }
    
    
    public async Task<LoginResponse> Login(LoginRequest request, CancellationToken token)
    {
        var user = await _userService.GetByEmail(request.Email, token);
        if (!await _userService.VerifyUser(request.Email, request.Password, token))
            throw new AuthenticationException("Wrong login or password");


        var tokens = await GetNewTokens(user, token);
        
        return new LoginResponse
        {
            User = user,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }

    public async Task<RegisterResponse> Register(CreateUserRequest request, CancellationToken token)
    {
        var oldUser = await _userService.GetByEmail(request.Email, token);
        if (oldUser != null)
            throw new ArgumentException("User already exist");

        var createUserResponse = await _userService.Create(request, token);

        var tokens = await GetNewTokens(createUserResponse.User, token);
        
        return new RegisterResponse
        {
            User = createUserResponse.User,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }

    public async Task<GetNewTokensResponse> GetNewTokens(string refreshToken, CancellationToken token)
    {
        var user = await VerifyRefreshToken(refreshToken, token);

        if (user != null)
        {
            if (await _tokenRepository.IsRefreshTokenActivated(refreshToken, token))
            {
                await _tokenRepository.ActivateAllTokens(user.Id, token);
            }
            else
            {
                await _tokenRepository.ActivateToken(refreshToken, token);
                return await GetNewTokens(user, token);
            }
        }

        return null;
    }

    private async Task<GetNewTokensResponse> GetNewTokens(User user, CancellationToken token)
    {
        var refreshToken = new JwtRefreshToken()
        {
            Token = GenerateToken(user,
                TokenType.Refresh),
            UserId = user.Id
        };
        await _tokenRepository.CreateRefreshToken(refreshToken, token);
        
        return new GetNewTokensResponse()
        {
            AccessToken = GenerateToken(user, TokenType.Access),
            RefreshToken = refreshToken.Token
        };
    }
    
    private string GenerateToken(User user, TokenType type)
    {
        var credentials = new SigningCredentials(_jwtSettings.GetSymmetricSecurityKey(),
            SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("type", type.ToString()),
            //new Claim("Username", user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var lifeTimeInSeconds = type == TokenType.Access
            ? _jwtSettings.TokenLifeTimeInSeconds
            : _jwtSettings.RefreshTokenLifeTimeInSeconds;
        
        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddSeconds(lifeTimeInSeconds),
            signingCredentials:credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private async Task<User> VerifyRefreshToken(string refreshToken, CancellationToken token)
    {
        var handler = new JwtSecurityTokenHandler();
        
        try
        {
            handler.ValidateToken(refreshToken,
                new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = _jwtSettings.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                },
                out var validatedToken);
            
            var jwtValidatedToken = validatedToken as JwtSecurityToken;

            var claim = jwtValidatedToken.Claims.FirstOrDefault(claim => claim.Type == "type");
            if (claim == null || claim.Value != TokenType.Refresh.ToString())
                return null;
            
            claim = jwtValidatedToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            if (claim == null)
                return null;

            var userId = long.Parse(claim.Value);
            var user = await _userService.GetById(userId, token);
            if (user == null)
                return null;
            
            return user;
        }
        catch
        {
            return null;
        }
    }
}