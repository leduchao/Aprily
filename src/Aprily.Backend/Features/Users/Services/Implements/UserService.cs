using Aprily.Backend.Database;
using Aprily.Backend.Entities;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Users.Services.Implements;

public class UserService(AppDbContext dbContext) : IUserService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<User?> CheckExistedEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }
}
