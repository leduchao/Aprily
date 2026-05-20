using Aprily.Application.Abstractions.Repositories;
using Aprily.Domain.Entities;
using Aprily.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
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
}