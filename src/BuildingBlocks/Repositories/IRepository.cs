using BuildingBlocks.Domain;

namespace BuildingBlocks.Repositories;

/// <summary>
/// 可写仓储接口，定义聚合根的增删改操作。
/// </summary>
public interface IRepository<TEntity, in TKey> : IReadOnlyRepository<TEntity, TKey>
    where TEntity : AggregateRoot, IEntity<TKey>
{
    /// <summary>
    /// 新增聚合根实例。
    /// </summary>
    /// <param name="entity">待持久化的聚合根对象。</param>
    /// <param name="autoSave">是否在操作后立即提交当前工作单元。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>持久化后的实体（可能带有数据库生成字段）。</returns>
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新聚合根实例。
    /// </summary>
    /// <param name="entity">待更新的聚合根对象。</param>
    /// <param name="autoSave">是否在操作后立即提交当前工作单元。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的实体。</returns>
    Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按实体实例删除聚合根。
    /// </summary>
    /// <param name="entity">待删除的实体对象。</param>
    /// <param name="autoSave">是否在操作后立即提交当前工作单元。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按主键删除聚合根。
    /// </summary>
    /// <param name="id">实体主键。</param>
    /// <param name="autoSave">是否在操作后立即提交当前工作单元。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);
}
