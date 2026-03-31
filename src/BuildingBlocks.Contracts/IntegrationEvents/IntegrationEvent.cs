namespace BuildingBlocks.Contracts.IntegrationEvents;

public abstract record IntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;

    public string? CorrelationId { get; init; }
}
