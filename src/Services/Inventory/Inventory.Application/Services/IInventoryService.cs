namespace Inventory.Application.Services;

public interface IInventoryService
{
    Task<int> AdjustAsync(Guid skuId, int delta, CancellationToken cancellationToken = default);
    Task<Inventory.Domain.Entities.StockItem?> GetBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default);
}
