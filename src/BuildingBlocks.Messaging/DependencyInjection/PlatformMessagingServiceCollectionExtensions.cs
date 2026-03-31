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

public static class PlatformMessagingServiceCollectionExtensions
{
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

        if (registry.Registrations.Count > 0)
        {
            services.AddHostedService<RabbitMqConsumerBackgroundService>();
        }

        return services;
    }
}
