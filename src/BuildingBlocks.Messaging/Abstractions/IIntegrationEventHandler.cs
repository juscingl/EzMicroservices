using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Messaging.Models;

namespace BuildingBlocks.Messaging.Abstractions;

public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IntegrationEvent
{
    Task HandleAsync(TEvent integrationEvent, IntegrationEventContext context, CancellationToken cancellationToken = default);
}
