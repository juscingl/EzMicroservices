namespace Inventory.Application.Services;

public interface IInventoryService
{
    Task<int> AdjustAsync(Guid skuId, int delta, CancellationToken cancellationToken = default);
}
