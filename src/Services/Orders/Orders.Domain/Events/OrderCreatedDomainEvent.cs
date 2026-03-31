using BuildingBlocks.Domain;

namespace Orders.Domain.Events;

public sealed record OrderCreatedDomainEvent(Guid OrderId, Guid CustomerId, decimal Amount)
    : DomainEvent(DateTimeOffset.UtcNow);
