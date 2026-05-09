using BuildingBlocks.Search.Abstractions;
using BuildingBlocks.Search.Options;
using BuildingBlocks.Search.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Search.DependencyInjection;

/// <summary>
/// 搜索能力注册扩展，统一接入 Elasticsearch 客户端与相关组件。
/// </summary>
public static class PlatformSearchServiceCollectionExtensions
{
    /// <summary>
    /// 注册平台搜索组件。
    /// </summary>
    public static IServiceCollection AddPlatformSearch(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<ElasticsearchOptions>()
            .Bind(configuration.GetSection(ElasticsearchOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IIndexNameResolver, IndexNameResolver>();
        services.AddHttpClient("elasticsearch", (serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
            httpClient.BaseAddress = new Uri(options.Uri);

            if (!string.IsNullOrWhiteSpace(options.UserName))
            {
                // 兼容通过 HttpClient 直接调用 ES REST API 的场景。
                var credentials = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes($"{options.UserName}:{options.Password}"));
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
            }
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
            var settings = new ElasticsearchClientSettings(new Uri(options.Uri));

            if (!string.IsNullOrWhiteSpace(options.UserName))
            {
                settings = settings.Authentication(new BasicAuthentication(options.UserName, options.Password ?? string.Empty));
            }

            return new ElasticsearchClient(settings);
        });

        return services;
    }
}
