using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Registration;

/// <summary>
/// 消息构建器，用于在启动阶段声明事件消费者。
/// </summary>
public sealed class PlatformMessagingBuilder(IServiceCollection services, IntegrationConsumerRegistry consumerRegistry)
{
    /// <summary>
    /// 注册一个事件消费者及其队列绑定关系。
    /// </summary>
    public PlatformMessagingBuilder AddConsumer<TEvent, THandler>(string queueName, string routingKey)
        where TEvent : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<TEvent>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName);
        ArgumentException.ThrowIfNullOrWhiteSpace(routingKey);

        services.AddScoped<THandler>();
        consumerRegistry.Add(new IntegrationConsumerRegistration(typeof(TEvent), typeof(THandler), queueName, routingKey));
        return this;
    }
}
