using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Messaging.Models;

namespace BuildingBlocks.Messaging.Abstractions;

/// <summary>
/// 集成事件处理器抽象，约束每类事件的消费入口。
/// </summary>
public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IntegrationEvent
{
    /// <summary>
    /// 处理收到的集成事件。
    /// </summary>
    Task HandleAsync(TEvent integrationEvent, IntegrationEventContext context, CancellationToken cancellationToken = default);
}
