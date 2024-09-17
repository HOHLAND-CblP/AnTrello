using AnTrello.Backend.Domain.Entities;

namespace AnTrello.Backend.Domain.Contracts.Dtos.UserService.GetProfile;

public class GetProfileResponse
{
    public User User { get; init; }
    public List<StatisticBody> Statistics { get; init; }

}

public class StatisticBody
{
    public string Label { get; init; }
    public int Value { get; init; }
}