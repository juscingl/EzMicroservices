namespace Orders.Domain.Entities;

/// <summary>
/// 订单项实体。
/// </summary>
public sealed class OrderItem
{
    /// <summary>
    /// 订单项标识。
    /// </summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// 商品标识。
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// 商品数量。
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// 商品单价。
    /// </summary>
    public decimal UnitPrice { get; private set; }

    private OrderItem()
    {
    }

    /// <summary>
    /// 创建订单项。
    /// </summary>
    /// <param name="productId">商品标识。</param>
    /// <param name="quantity">数量。</param>
    /// <param name="unitPrice">单价。</param>
    public OrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
