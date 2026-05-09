using BuildingBlocks.EntityFrameworkCore.Repositories;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Infrastructure.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

/// <summary>
/// 库存仓储实现。
/// </summary>
public sealed class InventoryRepository(InventoryDbContext dbContext)
    : Repository<InventoryDbContext, StockItem, Guid>(dbContext), IInventoryRepository
{
    /// <summary>
    /// 按 SKU 查询库存项。
    /// </summary>
    public Task<StockItem?> FindBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default)
    {
        return DbContext.StockItems.FirstOrDefaultAsync(stockItem => stockItem.SkuId == skuId, cancellationToken);
    }
}
