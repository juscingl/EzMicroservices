using System.Linq.Expressions;
using BuildingBlocks.Domain;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFrameworkCore.Repositories;

/// <summary>
/// 基于 EF Core 的通用仓储基类，封装聚合根的基础 CRUD 行为。
/// </summary>
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

    /// <summary>
    /// 当前仓储绑定的 DbContext。
    /// </summary>
    protected TDbContext DbContext { get; }

    /// <summary>
    /// 聚合根对应的数据集。
    /// </summary>
    protected DbSet<TEntity> DbSet { get; }

    /// <summary>
    /// 按主键尝试查询实体，不存在时返回 null。
    /// </summary>
    public virtual async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(entity => entity.Id!.Equals(id), cancellationToken);
    }

    /// <summary>
    /// 按主键查询实体，不存在时抛出实体不存在异常。
    /// </summary>
    public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(TEntity), id);
    }

    /// <summary>
    /// 查询全部实体列表。
    /// </summary>
    public virtual async Task<IReadOnlyList<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 按条件查询实体列表。
    /// </summary>
    public virtual async Task<IReadOnlyList<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 获取实体总数。
    /// </summary>
    public virtual async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(cancellationToken);
    }

    /// <summary>
    /// 判断是否存在满足条件的实体。
    /// </summary>
    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// 新增实体，并按配置决定是否立即提交。
    /// </summary>
    public virtual async Task<TEntity> InsertAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(autoSave, cancellationToken);
        return entity;
    }

    /// <summary>
    /// 更新实体，并按配置决定是否立即提交。
    /// </summary>
    public virtual async Task<TEntity> UpdateAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await SaveChangesAsync(autoSave, cancellationToken);
        return entity;
    }

    /// <summary>
    /// 删除指定实体，并按配置决定是否立即提交。
    /// </summary>
    public virtual async Task DeleteAsync(
        TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await SaveChangesAsync(autoSave, cancellationToken);
    }

    /// <summary>
    /// 按主键删除实体。若数据不存在则直接返回，不抛异常。
    /// </summary>
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
