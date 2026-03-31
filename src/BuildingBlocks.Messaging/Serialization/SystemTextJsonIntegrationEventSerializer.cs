using System.Text.Json;
using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Messaging.Models;

namespace BuildingBlocks.Messaging.Serialization;

internal sealed class SystemTextJsonIntegrationEventSerializer : IIntegrationEventSerializer
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ReadOnlyMemory<byte> SerializeEnvelope<TEvent>(TEvent integrationEvent)
        where TEvent : IntegrationEvent
    {
        var envelope = new IntegrationEventEnvelope
        {
            EventId = integrationEvent.EventId.ToString("N"),
            EventType = typeof(TEvent).FullName ?? typeof(TEvent).Name,
            OccurredOnUtc = integrationEvent.OccurredOnUtc,
            CorrelationId = integrationEvent.CorrelationId,
            Payload = JsonSerializer.Serialize(integrationEvent, JsonSerializerOptions)
        };

        return JsonSerializer.SerializeToUtf8Bytes(envelope, JsonSerializerOptions);
    }

    public IntegrationEventEnvelope DeserializeEnvelope(ReadOnlyMemory<byte> body)
    {
        return JsonSerializer.Deserialize<IntegrationEventEnvelope>(body.Span, JsonSerializerOptions)
            ?? throw new InvalidOperationException("Unable to deserialize integration event envelope.");
    }

    public object DeserializePayload(IntegrationEventEnvelope envelope, Type eventType)
    {
        return JsonSerializer.Deserialize(envelope.Payload, eventType, JsonSerializerOptions)
            ?? throw new InvalidOperationException($"Unable to deserialize integration event payload as {eventType.FullName}.");
    }
}
