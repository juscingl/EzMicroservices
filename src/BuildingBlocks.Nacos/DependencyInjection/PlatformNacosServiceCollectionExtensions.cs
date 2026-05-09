using BuildingBlocks.Nacos.HostedServices;
using BuildingBlocks.Nacos.Options;
using BuildingBlocks.Nacos.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Nacos.DependencyInjection;

/// <summary>
/// Nacos 能力注册扩展，统一接入配置与服务注册相关组件。
/// </summary>
public static class PlatformNacosServiceCollectionExtensions
{
    /// <summary>
    /// 注册平台 Nacos 组件。
    /// </summary>
    public static IServiceCollection AddPlatformNacos(
        this IServiceCollection services,
        IConfiguration configuration,
        string defaultServiceName)
    {
        services
            .AddOptions<NacosOptions>()
            .Bind(configuration.GetSection(NacosOptions.SectionName))
            .PostConfigure(options =>
            {
                if (string.IsNullOrWhiteSpace(options.ServiceName))
                {
                    options.ServiceName = defaultServiceName;
                }
            });

        services.AddHttpClient<INacosOpenApiClient, NacosOpenApiClient>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<NacosOptions>>().Value;
            httpClient.BaseAddress = new Uri(options.ServerAddress);
            httpClient.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);
        });

        services.AddHostedService<NacosServiceRegistrationHostedService>();
        return services;
    }
}
