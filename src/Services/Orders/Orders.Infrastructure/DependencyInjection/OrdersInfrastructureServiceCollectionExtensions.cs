using BuildingBlocks.Search.DependencyInjection;
using BuildingBlocks.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Search;
using Orders.Domain.Repositories;
using Orders.Infrastructure.EntityFrameworkCore.DbContexts;
using Orders.Infrastructure.Repositories;
using Orders.Infrastructure.Search;

namespace Orders.Infrastructure.DependencyInjection;

/// <summary>
/// 订单基础设施依赖注入扩展。
/// </summary>
public static class OrdersInfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// 注册订单模块基础设施依赖。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">应用配置。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddOrdersInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPlatformSearch(configuration);

        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<OrdersDbContext>());
        services.AddSingleton<ElasticOrderSearchService>();
        services.AddSingleton<IOrderSearchIndexer>(serviceProvider => serviceProvider.GetRequiredService<ElasticOrderSearchService>());
        services.AddSingleton<IOrderSearchReader>(serviceProvider => serviceProvider.GetRequiredService<ElasticOrderSearchService>());

        return services;
    }
}
