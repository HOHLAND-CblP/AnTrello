using System.IdentityModel.Tokens.Jwt;
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
        
        var accessToken = GenerateToken(user, _jwtSettings.TokenLifeTimeInSeconds,
            TokenType.Access);
        var refreshToken = new JwtRefreshToken()
        {
            Token = GenerateToken(user, _jwtSettings.RefreshTokenLifeTimeInSeconds,
                TokenType.Refresh),
            UserId = user.Id
        };
        
        await _tokenRepository.CreateRefreshToken(refreshToken, token);
        
        var response = new LoginResponse
        {
            User = user,
            Token = GenerateToken(user, _jwtSettings.TokenLifeTimeInSeconds, TokenType.Access),
            RefreshToken = GenerateToken(user, _jwtSettings.RefreshTokenLifeTimeInSeconds, TokenType.Refresh)
        };
        
        return response;
    }

    public async Task<RegisterResponse> Register(CreateUserRequest request, CancellationToken token)
    {
        var oldUser = await _userService.GetByEmail(request.Email, token);
        if (oldUser != null)
            throw new ArgumentException("User already exist");

        var createResponse = await _userService.Create(request, token);

        var accessToken = GenerateToken(createResponse.User, _jwtSettings.TokenLifeTimeInSeconds,
            TokenType.Access);
        var refreshToken = new JwtRefreshToken()
        {
            Token = GenerateToken(createResponse.User, _jwtSettings.RefreshTokenLifeTimeInSeconds,
                TokenType.Refresh),
            UserId = createResponse.User.Id
        };
        
        await _tokenRepository.CreateRefreshToken(refreshToken, token);
        
        var response = new RegisterResponse
        {
            User = createResponse.User,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
        return response;
    }

    public async Task<GetNewTokensResponse> GetNewTokens(string refreshToken, CancellationToken token)
    {
        var user = await _userService.GetById(1, token);
        
        if (VerifyRefreshToken(refreshToken))
            Console.WriteLine("1234");
        
        return new GetNewTokensResponse();
    }
    
    
    
    private string GenerateToken(User user, long tokenLifeTimeInSeconds, TokenType type)
    {
        var credentials = new SigningCredentials(_jwtSettings.GetSymmetricSecurityKey(),
            SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            new Claim("type", type.ToString().ToLower()),
            //new Claim("Username", user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        
        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires:new DateTime().AddSeconds(tokenLifeTimeInSeconds),
            signingCredentials:credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private bool VerifyRefreshToken(string refreshToken)
    {
        //var payload = JwtPayload.Base64UrlDeserialize(refreshToken);
        //payload.
        var handler = new JwtSecurityTokenHandler();

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
            out SecurityToken validatedToken);

        //handler.r()
        
        
            
        return true;
    }
}