using Aprily.Backend.Features.Users.Services.Implements;

namespace Aprily.Test;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher = new();

    [Fact]
    public void Verify_WithCorrectPassword_ReturnsTrue()
    {
        const string password = "correct-password";
        var hash = _passwordHasher.Hash(password);

        var result = _passwordHasher.Verify(password, hash);

        Assert.True(result);
    }

    [Fact]
    public void Verify_WithIncorrectPassword_ReturnsFalse()
    {
        var hash = _passwordHasher.Hash("correct-password");

        var result = _passwordHasher.Verify("incorrect-password", hash);

        Assert.False(result);
    }

    [Fact]
    public void Verify_WithLegacyHash_ReturnsTrue()
    {
        const string password = "correct-password";
        var currentHash = _passwordHasher.Hash(password);
        var legacyHash = string.Join(':', currentHash.Split(':').Skip(1));

        var result = _passwordHasher.Verify(password, legacyHash);

        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("100000:not-base64:not-base64")]
    public void Verify_WithMalformedHash_ReturnsFalse(string hash)
    {
        var result = _passwordHasher.Verify("correct-password", hash);

        Assert.False(result);
    }
}
