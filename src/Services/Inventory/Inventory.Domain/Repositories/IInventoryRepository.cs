using BuildingBlocks.Repositories;
using Inventory.Domain.Entities;

namespace Inventory.Domain.Repositories;

/// <summary>
/// 库存仓储接口。
/// </summary>
public interface IInventoryRepository : IRepository<StockItem, Guid>
{
    /// <summary>
    /// 按商品 SKU 查询库存项。
    /// </summary>
    Task<StockItem?> FindBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default);
}
