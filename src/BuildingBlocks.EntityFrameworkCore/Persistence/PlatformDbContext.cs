using BuildingBlocks.Auditing;
using BuildingBlocks.EntityFrameworkCore.Auditing;
using BuildingBlocks.Uow;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFrameworkCore.Persistence;

public abstract class PlatformDbContext<TDbContext>(
    DbContextOptions<TDbContext> options,
    ICurrentUserAccessor? currentUserAccessor = null)
    : DbContext(options), IUnitOfWork
    where TDbContext : DbContext
{
    protected ICurrentUserAccessor CurrentUserAccessor { get; } = currentUserAccessor ?? NullCurrentUserAccessor.Instance;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyPlatformConventions();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.ApplyPlatformAuditing(CurrentUserAccessor);
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ApplyPlatformAuditing(CurrentUserAccessor);
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.ApplyPlatformAuditing(CurrentUserAccessor);
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
