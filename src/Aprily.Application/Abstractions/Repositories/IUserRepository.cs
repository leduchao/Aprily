using Aprily.Domain.Entities;

namespace Aprily.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByEntityId(Guid entityId);
    Task<User?> GetUserByEmail(string email);

    Task<User> AddUser(User user, CancellationToken ct);
    Task<int> UpdateUser(User user, CancellationToken ct);

    Task<RefreshToken?> GetRefreshToken(string refreshToken, CancellationToken cancellation);
    Task<IList<RefreshToken>> GetRefreshTokensByUserId(int userId, CancellationToken cancellation);
    Task<int> AddRefreshToken(RefreshToken refreshToken, CancellationToken ct);
    Task<int> RevokeAllRefreshTokens(IList<RefreshToken> refreshTokens, CancellationToken cancellation);
    Task<int> RevokeAllRefreshTokens(int userId, CancellationToken cancellation);
}