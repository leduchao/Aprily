using Aprily.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}
