using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Messaging.Models;

namespace BuildingBlocks.Messaging.Serialization;

public interface IIntegrationEventSerializer
{
    ReadOnlyMemory<byte> SerializeEnvelope<TEvent>(TEvent integrationEvent)
        where TEvent : IntegrationEvent;

    IntegrationEventEnvelope DeserializeEnvelope(ReadOnlyMemory<byte> body);

    object DeserializePayload(IntegrationEventEnvelope envelope, Type eventType);
}
