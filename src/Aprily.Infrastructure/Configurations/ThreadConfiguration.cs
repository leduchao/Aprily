using Aprily.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aprily.Infrastructure.Configurations;

internal class ThreadConfiguration : IEntityTypeConfiguration<Domain.Entities.Thread>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Thread> builder)
    {
        builder.ToTable(nameof(AppDbContext.Threads), Schemas.Chat);

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.EntityId).IsUnique();

        builder.HasOne(t => t.Creator)
            .WithMany(u => u.Threads)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
