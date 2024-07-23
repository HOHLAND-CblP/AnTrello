using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AnTrello.Backend.Domain.Contracts.Dtos.User.Login;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;
using AnTrello.Backend.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AnTrello.Backend.Domain.Services;

public class AuthService : IAuthService
{
    private readonly HashSettings _hashSettings;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IOptions<HashSettings> hashSettings, IOptions<JwtSettings> jwtSettings)
    {
        _hashSettings = hashSettings.Value;
        _jwtSettings = jwtSettings.Value;
    }
    
    

     public async Task<LoginResponse> Login(LoginRequest request, CancellationToken token)
     {  
          return new LoginResponse();
     }
     
     
    private string GenerateJWT(User user)
    {
        
        var credentials = new SigningCredentials(_jwtSettings.GetSymmetricSecurityKey(),
            SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            //new Claim("Username", user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires:DateTime.UtcNow.AddSeconds(_jwtSettings.TokenLifeTimeInSeconds),
            signingCredentials:credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    
    
    private string CreatePbkdf2Hash(string password)
    {
        byte[] salt = GenerateSalt();
        string hashPassword = MakePbkdf2HashPassword(password, salt);
        
        return hashPassword;
    }

    
    private bool CheckPassword(string password, string hash)
    {
        byte[] salt = GetSalt(hash);

        string hashPassword = MakePbkdf2HashPassword(password, salt);

        return hash == hashPassword;
    }

    private byte[] GetSalt(string hash)
    {
        byte[] hashBytes = Convert.FromBase64String(hash);
        byte[] salt = new byte[_hashSettings.SaltLenght];

        for (int i = 0; i < _hashSettings.SaltLenght; i++)
            salt[i] = hashBytes[hashBytes.Length - _hashSettings.SaltLenght + i];
        
        return salt;
    }
    
    
    private string MakePbkdf2HashPassword(string password, byte[] salt)
    {
        byte[] passwordBytes  = Encoding.UTF8.GetBytes(password);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, _hashSettings.Pbkdf2Iterations, HashAlgorithmName.FromOid(_hashSettings.HashAlgorithm), _hashSettings.HashLength);
        byte[] hashWithSaltBytes = new byte[hash.Length + salt.Length];
        
        for (int i = 0; i < hash.Length; i++) 
            hashWithSaltBytes[i] = hash[i]; 

        for (int i = hash.Length; i < hashWithSaltBytes.Length; i++)
            hashWithSaltBytes[i] = salt[i - hash.Length];
        
        return Convert.ToBase64String(hashWithSaltBytes);
    }

    private byte[] GenerateSalt()
    {
        byte[] salt = new byte[_hashSettings.SaltLenght];
        
        var rngRand = new RNGCryptoServiceProvider();
        rngRand.GetBytes(salt);

        return salt;
    }
}