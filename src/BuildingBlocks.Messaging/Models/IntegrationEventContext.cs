namespace BuildingBlocks.Messaging.Models;

public sealed record IntegrationEventContext(
    string EventId,
    string? CorrelationId,
    DateTime OccurredOnUtc,
    string RoutingKey);
