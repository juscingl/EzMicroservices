using BuildingBlocks.Auditing;
using BuildingBlocks.EntityFrameworkCore.Auditing;
using BuildingBlocks.Uow;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFrameworkCore.Persistence;

/// <summary>
/// 平台级 DbContext 基类，统一接入审计字段填充与模型约定。
/// </summary>
public abstract class PlatformDbContext<TDbContext>(
    DbContextOptions<TDbContext> options,
    ICurrentUserAccessor? currentUserAccessor = null)
    : DbContext(options), IUnitOfWork
    where TDbContext : DbContext
{
    /// <summary>
    /// 当前用户访问器。缺省情况下使用空对象，避免审计流程判空分支。
    /// </summary>
    protected ICurrentUserAccessor CurrentUserAccessor { get; } = currentUserAccessor ?? NullCurrentUserAccessor.Instance;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyPlatformConventions();
    }

    /// <summary>
    /// 保存变更前自动写入审计字段，然后交由 EF Core 提交。
    /// </summary>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.ApplyPlatformAuditing(CurrentUserAccessor);
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// 异步保存变更前自动写入审计字段。
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ApplyPlatformAuditing(CurrentUserAccessor);
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 异步保存变更（可控制是否接受全部变更），并自动写入审计字段。
    /// </summary>
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.ApplyPlatformAuditing(CurrentUserAccessor);
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
