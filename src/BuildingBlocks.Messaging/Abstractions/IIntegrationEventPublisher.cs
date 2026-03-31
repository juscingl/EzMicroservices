using BuildingBlocks.Contracts.IntegrationEvents;

namespace BuildingBlocks.Messaging.Abstractions;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, string routingKey, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent;
}
