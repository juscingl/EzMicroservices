using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFrameworkCore.Auditing;

/// <summary>
/// DbContext 审计扩展，负责在提交前统一填充创建/修改/删除审计字段。
/// </summary>
public static class DbContextAuditingExtensions
{
    /// <summary>
    /// 对当前变更集应用平台审计规则。
    /// </summary>
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
                        // 软删除对象统一转换为更新操作，避免物理删除。
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
