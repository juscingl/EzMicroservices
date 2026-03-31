using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Registration;

public sealed class PlatformMessagingBuilder(IServiceCollection services, IntegrationConsumerRegistry consumerRegistry)
{
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
