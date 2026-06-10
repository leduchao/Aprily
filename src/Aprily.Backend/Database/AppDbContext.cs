using Aprily.Backend.Entities;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}