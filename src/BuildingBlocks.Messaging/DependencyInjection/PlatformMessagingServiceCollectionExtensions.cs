using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Connection;
using BuildingBlocks.Messaging.Consumers;
using BuildingBlocks.Messaging.Options;
using BuildingBlocks.Messaging.Publishers;
using BuildingBlocks.Messaging.Registration;
using BuildingBlocks.Messaging.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.DependencyInjection;

/// <summary>
/// 消息能力注册扩展，统一注入 RabbitMQ 发布/消费基础设施。
/// </summary>
public static class PlatformMessagingServiceCollectionExtensions
{
    /// <summary>
    /// 注册平台消息组件，并按需启动后台消费者。
    /// </summary>
    public static IServiceCollection AddPlatformMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<PlatformMessagingBuilder>? configure = null)
    {
        services
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateOnStart();

        var registry = new IntegrationConsumerRegistry();
        configure?.Invoke(new PlatformMessagingBuilder(services, registry));

        services.AddSingleton(registry);
        services.AddSingleton<RabbitMqConnectionProvider>();
        services.AddSingleton<IIntegrationEventSerializer, SystemTextJsonIntegrationEventSerializer>();
        services.AddSingleton<IIntegrationEventPublisher, RabbitMqIntegrationEventPublisher>();

        // 只有声明了消费者时才启动后台消费服务。
        if (registry.Registrations.Count > 0)
        {
            services.AddHostedService<RabbitMqConsumerBackgroundService>();
        }

        return services;
    }
}
