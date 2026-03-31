namespace BuildingBlocks.Contracts.IntegrationEvents;

public sealed record OrderCreatedIntegrationEvent : IntegrationEvent
{
    public required Guid OrderId { get; init; }

    public required Guid CustomerId { get; init; }

    public required decimal TotalAmount { get; init; }

    public required string Currency { get; init; }

    public required IReadOnlyCollection<OrderCreatedLine> Lines { get; init; }
}

public sealed record OrderCreatedLine(Guid ProductId, int Quantity, decimal UnitPrice);
