using Aprily.Domain.Entities;

namespace Aprily.Application.Abstractions.Services;

public interface ITokenProvider
{
    public string GenerateToken(User user);
    public Guid? ValidateToken(string token);
}