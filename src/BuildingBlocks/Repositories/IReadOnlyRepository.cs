using System.Linq.Expressions;
using BuildingBlocks.Domain;

namespace BuildingBlocks.Repositories;

public interface IReadOnlyRepository<TEntity, in TKey>
    where TEntity : AggregateRoot, IEntity<TKey>
{
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
