using Aprily.Backend.Entities;

namespace Aprily.Backend.Features.Users.Services.Abstractions;

public interface ITokenProvider
{
    public string GenerateRefreshToken();
    public string GenerateAccessToken(User user);
    public Guid? ValidateToken(string token);
}