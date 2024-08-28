using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Dtos.UserService.GetProfile;

public class GetProfileResponse
{
    public User User { get; init; }
    public Dictionary<string, int> Statistics { get; init; }
}   
