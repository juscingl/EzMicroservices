using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFrameworkCore.Auditing;

public static class DbContextAuditingExtensions
{
    public static void ApplyPlatformAuditing(this DbContext dbContext, ICurrentUserAccessor? currentUserAccessor = null)
    {
        dbContext.ChangeTracker.DetectChanges();

        var currentUser = currentUserAccessor ?? NullCurrentUserAccessor.Instance;
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyCreationAudit(entry.Entity, now, currentUser.UserId);
                    break;
                case EntityState.Modified:
                    ProtectCreationAudit(entry);
                    ApplyModificationAudit(entry.Entity, now, currentUser.UserId);
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        entry.State = EntityState.Modified;
                        softDelete.IsDeleted = true;
                        ApplyDeletionAudit(entry.Entity, now, currentUser.UserId);
                        ApplyModificationAudit(entry.Entity, now, currentUser.UserId);
                    }
                    break;
            }
        }
    }

    private static void ApplyCreationAudit(object entity, DateTimeOffset now, Guid? userId)
    {
        if (entity is IHasCreationTime creationTime && creationTime.CreationTime == default)
        {
            creationTime.CreationTime = now;
        }

        if (entity is IMayHaveCreator creator && !creator.CreatorId.HasValue)
        {
            creator.CreatorId = userId;
        }
    }

    private static void ApplyModificationAudit(object entity, DateTimeOffset now, Guid? userId)
    {
        if (entity is IModificationAuditedObject modificationAuditedObject)
        {
            modificationAuditedObject.LastModificationTime = now;
            modificationAuditedObject.LastModifierId = userId;
        }
    }

    private static void ApplyDeletionAudit(object entity, DateTimeOffset now, Guid? userId)
    {
        if (entity is IDeletionAuditedObject deletionAuditedObject)
        {
            deletionAuditedObject.DeletionTime = now;
            deletionAuditedObject.DeleterId = userId;
        }
    }

    private static void ProtectCreationAudit(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        if (entry.Entity is IHasCreationTime)
        {
            entry.Property(nameof(IHasCreationTime.CreationTime)).IsModified = false;
        }

        if (entry.Entity is IMayHaveCreator)
        {
            entry.Property(nameof(IMayHaveCreator.CreatorId)).IsModified = false;
        }
    }
}
