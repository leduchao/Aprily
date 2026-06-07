using Aprily.Domain.Entities;
using Aprily.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aprily.Infrastructure.Configurations;

internal class ThreadParticipantConfiguration : IEntityTypeConfiguration<ThreadParticipant>
{
    public void Configure(EntityTypeBuilder<ThreadParticipant> builder)
    {
        builder.ToTable(nameof(AppDbContext.ThreadParticipants), Schemas.Chat);

        builder.HasKey(p => new { p.ThreadId, p.UserId });

        builder.HasOne(p => p.Thread)
            .WithMany(t => t.Participants)
            .HasForeignKey(p => p.ThreadId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.User)
            .WithMany(u => u.ThreadParticipants)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
