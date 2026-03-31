using BuildingBlocks.Repositories;
using Inventory.Domain.Entities;

namespace Inventory.Domain.Repositories;

public interface IInventoryRepository : IRepository<StockItem, Guid>
{
    Task<StockItem?> FindBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default);
}
