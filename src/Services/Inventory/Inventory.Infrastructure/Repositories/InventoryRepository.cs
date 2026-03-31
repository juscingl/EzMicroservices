using BuildingBlocks.EntityFrameworkCore.Repositories;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Infrastructure.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public sealed class InventoryRepository(InventoryDbContext dbContext)
    : Repository<InventoryDbContext, StockItem, Guid>(dbContext), IInventoryRepository
{
    public Task<StockItem?> FindBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default)
    {
        return DbContext.StockItems.FirstOrDefaultAsync(stockItem => stockItem.SkuId == skuId, cancellationToken);
    }
}
