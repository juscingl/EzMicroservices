using BuildingBlocks.Domain;
using Orders.Domain.Events;

namespace Orders.Domain.Entities;

/// <summary>
/// 订单聚合根。
/// </summary>
public sealed class Order : FullAuditedAggregateRoot
{
    private readonly List<OrderItem> _items = new();

    /// <summary>
    /// 客户标识。
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// 订单项只读集合。
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    /// <summary>
    /// 订单总金额。
    /// </summary>
    public decimal Total => _items.Sum(item => item.Quantity * item.UnitPrice);

    private Order()
    {
    }

    /// <summary>
    /// 创建订单实体。
    /// </summary>
    /// <param name="customerId">客户标识。</param>
    /// <param name="items">订单项集合。</param>
    public Order(Guid customerId, IEnumerable<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        _items.AddRange(items);
        AddDomainEvent(new OrderCreatedDomainEvent(Id, CustomerId, Total));
    }

    /// <summary>
    /// 添加订单项。
    /// </summary>
    /// <param name="productId">商品标识。</param>
    /// <param name="quantity">数量。</param>
    /// <param name="unitPrice">单价。</param>
    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        _items.Add(new OrderItem(productId, quantity, unitPrice));
    }
}
