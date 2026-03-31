using BuildingBlocks.Uow;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;

namespace Inventory.Application.Services;

public sealed class InventoryService(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork) : IInventoryService
{
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
}
