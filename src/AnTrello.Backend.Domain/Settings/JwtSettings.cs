using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AnTrello.Backend.Domain.Settings;

public class JwtSettings
{
    public string Issuer { get; init; } = "Issuer";
    public string Audience { get; init; } = "Audience";
    public string SecretKey { get; init; } = "djoajd21d0j29djj10j1902dj901djn0320j";
    public long TokenLifeTimeInSeconds { get; init; } = 604800;

    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
    }

    public JwtSettings(){}
    public JwtSettings(IConfiguration config)
    {
        string opt;
        if (!string.IsNullOrWhiteSpace(opt = config["Issuer"]))
        {
            Issuer = opt;
        }
        if (!string.IsNullOrWhiteSpace(opt = config["Audience"]))
        {
            Audience = opt;
        }
        if (!string.IsNullOrWhiteSpace(opt = config["SecretKey"]))
        {
            SecretKey = opt;
        }
        if (!string.IsNullOrWhiteSpace(opt = config["TokenLifeTimeInSeconds"]))
        {
            if (long.TryParse(opt, out long time))
                TokenLifeTimeInSeconds = time;
            else
                throw new ArgumentException("TokenLifeTimeInSeconds must be long (Int64)");
        }
    }
}