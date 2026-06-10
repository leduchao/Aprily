namespace Aprily.Backend.Features.Users.Services.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hashedPassword);
}