namespace Inventory.Application.Services;

/// <summary>
/// 库存应用服务接口。
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// 调整指定 SKU 的库存增量。
    /// </summary>
    Task<int> AdjustAsync(Guid skuId, int delta, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按 SKU 查询库存明细。
    /// </summary>
    Task<Inventory.Domain.Entities.StockItem?> GetBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default);
}
