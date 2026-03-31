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

public static class OrdersInfrastructureServiceCollectionExtensions
{
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
