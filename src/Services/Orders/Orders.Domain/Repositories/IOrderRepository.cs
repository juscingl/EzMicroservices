using BuildingBlocks.Repositories;
using Orders.Domain.Entities;

namespace Orders.Domain.Repositories;

public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<Order?> FindWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
