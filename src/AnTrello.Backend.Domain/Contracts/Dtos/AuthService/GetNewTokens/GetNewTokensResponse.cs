namespace AnTrello.Backend.Domain.Contracts.Dtos.AuthService.GetNewTokens;

public class GetNewTokensResponse
{
    public string Token { get; init; }
    public string RefreshToken { get; set; }
}