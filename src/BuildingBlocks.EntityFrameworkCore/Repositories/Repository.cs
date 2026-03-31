using System.Linq.Expressions;
using BuildingBlocks.Domain;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFrameworkCore.Repositories;

public abstract class Repository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
    where TDbContext : DbContext
    where TEntity : AggregateRoot, IEntity<TKey>
    where TKey : notnull
{
    protected Repository(TDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    protected TDbContext DbContext { get; }

    protected DbSet<TEntity> DbSet { get; }

    public virtual async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(entity => entity.Id!.Equals(id), cancellationToken);
    }

    public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(TEntity), id);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity> InsertAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(autoSave, cancellationToken);
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await SaveChangesAsync(autoSave, cancellationToken);
        return entity;
    }

    public virtual async Task DeleteAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await SaveChangesAsync(autoSave, cancellationToken);
    }

    public virtual async Task DeleteAsync(
        TKey id,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        await DeleteAsync(entity, autoSave, cancellationToken);
    }

    protected virtual async Task SaveChangesAsync(bool autoSave, CancellationToken cancellationToken)
    {
        if (!autoSave)
        {
            return;
        }

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
