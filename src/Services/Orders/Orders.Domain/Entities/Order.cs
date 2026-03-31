using BuildingBlocks.Domain;
using Orders.Domain.Events;

namespace Orders.Domain.Entities;

public sealed class Order : FullAuditedAggregateRoot
{
    private readonly List<OrderItem> _items = new();

    public Guid CustomerId { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal Total => _items.Sum(item => item.Quantity * item.UnitPrice);

    private Order()
    {
    }

    public Order(Guid customerId, IEnumerable<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        _items.AddRange(items);
        AddDomainEvent(new OrderCreatedDomainEvent(Id, CustomerId, Total));
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        _items.Add(new OrderItem(productId, quantity, unitPrice));
    }
}
