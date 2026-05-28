using Aprily.Application.Abstractions.Repositories;
using Aprily.Domain.Entities;
using Aprily.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<User?> GetUserByEntityId(Guid entityId)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.EntityId.Equals(entityId));
        return user;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var user = await db.Users.FirstOrDefaultAsync(u =>
            EF.Functions.ILike(u.Email, email) || EF.Functions.ILike(u.Username, email));

        return user;
    }

    public async Task<User> AddUser(User user, CancellationToken ct)
    {
        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
        return user;
    }

    public async Task<int> UpdateUser(User user, CancellationToken ct)
    {
        db.Users.Update(user);
        return await db.SaveChangesAsync(ct);
    }

    public async Task<RefreshToken?> GetRefreshToken(string refreshToken, CancellationToken cancellation)
    {
        return await db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token.Equals(refreshToken), cancellation);
    }

    public async Task<IList<RefreshToken>> GetRefreshTokensByUserId(int userId, CancellationToken cancellationToken)
    {
        var refreshTokens = db.RefreshTokens.Where(r => 
            !r.IsRevoked 
            && r.ExpiryDate > DateTime.UtcNow
            && !string.IsNullOrEmpty(r.ReplacedByToken));

        return await refreshTokens.ToListAsync(cancellationToken);
    }

    public async Task<int> AddRefreshToken(RefreshToken refreshToken, CancellationToken ct)
    {
        await db.RefreshTokens.AddAsync(refreshToken, ct);
        return await db.SaveChangesAsync(ct);
    }

    public async Task<int> RevokeAllRefreshTokens(IList<RefreshToken> refreshTokens, CancellationToken cancellation)
    {
        var tokenIds = refreshTokens.Select(r => r.Id);

        return await db.RefreshTokens
            .Where(r => tokenIds.Contains(r.Id))
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsRevoked, true), cancellation);
    }

    public async Task<int> RevokeAllRefreshTokens(int userId, CancellationToken cancellation)
    {
        return await db.RefreshTokens
            .Where(r => r.UserId == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsRevoked, true), cancellation);
    }
}