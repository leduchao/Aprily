using Aprily.Domain.Entities;

namespace Aprily.Application.Abstractions.Services;

public interface ITokenProvider
{
    public string GenerateRefreshToken();
    public string GenerateAccessToken(User user);
    public Guid? ValidateToken(string token);
}