namespace BuildingBlocks.Messaging.Models;

/// <summary>
/// 事件消费上下文，补充消息元数据供处理器使用。
/// </summary>
public sealed record IntegrationEventContext(
    string EventId,
    string? CorrelationId,
    DateTime OccurredOnUtc,
    string RoutingKey);
