using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Aprily.Backend.Database.Interceptors;

public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateAuditFields(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        var utcNow = DateTime.UtcNow;

        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Added &&
                entry.State != EntityState.Modified)
            {
                continue;
            }

            var hasCreatedAt = entry.Properties.Any(p => p.Metadata.Name == "CreatedAt");
            var hasUpdatedAt = entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt");

            if (!hasUpdatedAt)
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                if (hasCreatedAt)
                {
                    entry.Property("CreatedAt").CurrentValue = utcNow;
                }

                entry.Property("UpdatedAt").CurrentValue = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                if (hasCreatedAt)
                {
                    entry.Property("CreatedAt").IsModified = false;
                }

                entry.Property("UpdatedAt").CurrentValue = utcNow;
            }
        }
    }
}
