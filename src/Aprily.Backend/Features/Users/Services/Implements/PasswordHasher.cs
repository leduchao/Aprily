using System.Security.Cryptography;

namespace Aprily.Backend.Features.Users.Services.Implements;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private const char Delimiter = ':';
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return $"{Iterations}{Delimiter}{Convert.ToBase64String(salt)}{Delimiter}{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
        {
            return false;
        }

        var parts = hashedPassword.Split(Delimiter);

        string encodedSalt;
        string encodedKey;

        if (parts.Length == 3 && int.TryParse(parts[0], out int iterations))
        {
            encodedSalt = parts[1];
            encodedKey = parts[2];
        }
        else if (parts.Length == 2)
        {
            iterations = Iterations;
            encodedSalt = parts[0];
            encodedKey = parts[1];
        }
        else
        {
            return false;
        }

        if (iterations <= 0)
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(encodedSalt);
            var key = Convert.FromBase64String(encodedKey);

            if (salt.Length != SaltSize || key.Length != KeySize)
            {
                return false;
            }

            var computedKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                Algorithm,
                KeySize);

            return CryptographicOperations.FixedTimeEquals(computedKey, key);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
