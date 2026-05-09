using BuildingBlocks.Contracts.IntegrationEvents;

namespace BuildingBlocks.Messaging.Abstractions;

/// <summary>
/// 集成事件发布器抽象。
/// </summary>
public interface IIntegrationEventPublisher
{
    /// <summary>
    /// 发布集成事件到消息中间件。
    /// </summary>
    Task PublishAsync<TEvent>(TEvent integrationEvent, string routingKey, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent;
}
