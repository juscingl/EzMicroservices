using BuildingBlocks.Domain;

namespace Inventory.Domain.Entities;

public sealed class StockItem : FullAuditedAggregateRoot
{
    public Guid SkuId { get; private set; }

    public int Quantity { get; private set; }

    private StockItem()
    {
    }

    public StockItem(Guid skuId, int quantity)
    {
        Id = Guid.NewGuid();
        SkuId = skuId;
        Quantity = quantity;
    }

    public void Adjust(int delta)
    {
        Quantity += delta;
    }
}
