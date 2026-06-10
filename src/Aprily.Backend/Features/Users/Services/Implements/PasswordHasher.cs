using System.Security.Cryptography;

using Aprily.Backend.Features.Users.Services.Abstractions;

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

        return $"{Convert.ToBase64String(salt)}{Delimiter}{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            return false;
        }

        var parts = hashedPassword.Split(Delimiter);
        if (parts.Length != 2)
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[0]);
        var key = Convert.FromBase64String(parts[1]);
        if (key.Length != KeySize)
        {
            return false;
        }

        var computedKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return CryptographicOperations.FixedTimeEquals(computedKey, key);
    }
}