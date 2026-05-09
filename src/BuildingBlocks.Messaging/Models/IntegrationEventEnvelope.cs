namespace BuildingBlocks.Messaging.Models;

/// <summary>
/// 集成事件消息信封，统一承载事件元信息与序列化负载。
/// </summary>
public sealed record IntegrationEventEnvelope
{
    /// <summary>
    /// 事件唯一标识。
    /// </summary>
    public required string EventId { get; init; }

    /// <summary>
    /// 事件类型全名，用于消费端反序列化。
    /// </summary>
    public required string EventType { get; init; }

    /// <summary>
    /// 事件发生时间（UTC）。
    /// </summary>
    public required DateTime OccurredOnUtc { get; init; }

    /// <summary>
    /// 链路关联标识。
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// 事件 JSON 负载。
    /// </summary>
    public required string Payload { get; init; }
}
