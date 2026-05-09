namespace BuildingBlocks.Uow;

/// <summary>
/// 表示IUnitOfWork。
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
