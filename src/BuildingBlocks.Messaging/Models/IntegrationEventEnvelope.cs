namespace BuildingBlocks.Messaging.Models;

public sealed record IntegrationEventEnvelope
{
    public required string EventId { get; init; }

    public required string EventType { get; init; }

    public required DateTime OccurredOnUtc { get; init; }

    public string? CorrelationId { get; init; }

    public required string Payload { get; init; }
}
