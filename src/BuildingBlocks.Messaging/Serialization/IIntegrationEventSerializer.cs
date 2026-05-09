using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Messaging.Models;

namespace BuildingBlocks.Messaging.Serialization;

/// <summary>
/// 集成事件序列化抽象，负责消息信封与业务负载的编解码。
/// </summary>
public interface IIntegrationEventSerializer
{
    /// <summary>
    /// 将事件序列化为消息信封字节数组。
    /// </summary>
    ReadOnlyMemory<byte> SerializeEnvelope<TEvent>(TEvent integrationEvent)
        where TEvent : IntegrationEvent;

    /// <summary>
    /// 从消息字节中反序列化信封。
    /// </summary>
    IntegrationEventEnvelope DeserializeEnvelope(ReadOnlyMemory<byte> body);

    /// <summary>
    /// 按指定事件类型反序列化业务负载。
    /// </summary>
    object DeserializePayload(IntegrationEventEnvelope envelope, Type eventType);
}
