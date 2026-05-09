using BuildingBlocks.Uow;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;

namespace Inventory.Application.Services;

/// <summary>
/// 库存应用服务实现，负责库存调整与查询。
/// </summary>
public sealed class InventoryService(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork) : IInventoryService
{
    /// <summary>
    /// 调整库存：不存在则创建库存项，存在则累加/扣减。
    /// </summary>
    public async Task<int> AdjustAsync(Guid skuId, int delta, CancellationToken cancellationToken = default)
    {
        var stockItem = await inventoryRepository.FindBySkuIdAsync(skuId, cancellationToken);
        if (stockItem is null)
        {
            stockItem = new StockItem(skuId, delta);
            await inventoryRepository.InsertAsync(stockItem, cancellationToken: cancellationToken);
        }
        else
        {
            stockItem.Adjust(delta);
            await inventoryRepository.UpdateAsync(stockItem, cancellationToken: cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return stockItem.Quantity;
    }

    /// <summary>
    /// 查询指定 SKU 的库存项。
    /// </summary>
    public Task<StockItem?> GetBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default)
    {
        return inventoryRepository.FindBySkuIdAsync(skuId, cancellationToken);
    }
}
