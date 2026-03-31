using BuildingBlocks.Domain;

namespace BuildingBlocks.Repositories;

public interface IRepository<TEntity, in TKey> : IReadOnlyRepository<TEntity, TKey>
    where TEntity : AggregateRoot, IEntity<TKey>
{
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);
}
