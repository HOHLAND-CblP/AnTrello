using System.Security.Cryptography;
using System.Text;
using AnTrello.Backend.Domain.Contracts.Dtos.UserService.Create;
using AnTrello.Backend.Domain.Contracts.Dtos.UserService.Update;
using AnTrello.Backend.Domain.Contracts.Repositories;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;
using AnTrello.Backend.Domain.Settings;
using Microsoft.Extensions.Options;

namespace AnTrello.Backend.Domain.Services;

internal class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly HashSettings _hashSettings;

    public UserService(IUserRepository repository, IOptions<HashSettings> hashSettings)
    {
        _repository = repository;
        _hashSettings = hashSettings.Value;
    }

    
    public async Task<CreateUserResponse> Create(CreateUserRequest request, CancellationToken token)
    {
        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            Password = CreatePbkdf2Hash(request.Password),
            BreakInterval = 50,
            IntervalsCount = 10,
            WorkInterval = 7,
            CreatedAt = DateTime.UtcNow
        };
        
        user.Id = await _repository.CreateUser(user, token); 

        var response = new CreateUserResponse()
        {
            User = user
        };

        return response;
    }

    public async Task<User> GetById(long id, CancellationToken token)
    {
        var user = await _repository.GetById(id, token);
        return user;
    }

    public async Task<User> GetByEmail(string email, CancellationToken token)
    {
        var user = await _repository.GetByEmail(email, token);
        return user;
    }
    
    public async Task<bool> VerifyUser(string email, string password, CancellationToken token)
    {
        var user = await _repository.GetByEmail(email, token);
        return user != null && CheckPassword(password, user.Password);
    }

    public async Task<User> Update(UpdateUserRequest request, CancellationToken token)
    {
        var user = await _repository.GetById(request.User.Id, token);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user = await _repository.UpdateUser(request.User, token);

        return user;
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

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, _hashSettings.Pbkdf2Iterations, HashAlgorithmName.SHA512, _hashSettings.HashLength);
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