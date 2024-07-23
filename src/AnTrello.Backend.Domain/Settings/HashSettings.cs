using Microsoft.Extensions.Configuration;

namespace AnTrello.Backend.Domain.Settings;

public class HashSettings
{
    public int SaltLenght { get; init; } = 64;
    public int HashLength { get; init; } = 64;
    public int Pbkdf2Iterations { get; init; } = 1000;
    public string HashAlgorithm { get; init; } = "SHA512";
    
    public HashSettings(){}
    public HashSettings(IConfiguration config)
    {
        string opt;
        if (!string.IsNullOrWhiteSpace(opt = config["SaltLenght"]))
        {
            if (int.TryParse(opt, out int time))
                SaltLenght = time;
            else
                throw new ArgumentException("TokenLifeTimeInSeconds must be int (Int32)");
        }
        if (!string.IsNullOrWhiteSpace(opt = config["HashLength"]))
        {
            if (int.TryParse(opt, out int time))
                HashLength = time;
            else
                throw new ArgumentException("TokenLifeTimeInSeconds must be int (Int32)");
        }
        if (!string.IsNullOrWhiteSpace(opt = config["Pbkdf2Iterations"]))
        {
            if (int.TryParse(opt, out int time))
                Pbkdf2Iterations = time;
            else
                throw new ArgumentException("TokenLifeTimeInSeconds must be int (Int32)");
        }
        if (!string.IsNullOrWhiteSpace(opt = config["HashAlgorithm"]))
        { 
            HashAlgorithm = opt;
        }
    }
}