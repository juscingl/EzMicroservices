using System.Linq.Expressions;
using BuildingBlocks.Domain;

namespace BuildingBlocks.Repositories;

/// <summary>
/// 只读仓储接口，提供聚合根查询能力。
/// </summary>
public interface IReadOnlyRepository<TEntity, in TKey>
    where TEntity : AggregateRoot, IEntity<TKey>
{
    /// <summary>
    /// 尝试按主键查询实体，不存在时返回 null。
    /// </summary>
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按主键查询实体，不存在时应抛出异常。
    /// </summary>
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询全部实体列表。
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 按条件查询实体列表。
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体总数。
    /// </summary>
    Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断是否存在满足条件的数据。
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
