using BuildingBlocks.Uow;
using Inventory.Domain.Repositories;
using Inventory.Infrastructure.EntityFrameworkCore.DbContexts;
using Inventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure.DependencyInjection;

/// <summary>
/// 库存模块基础设施依赖注入扩展。
/// </summary>
public static class InventoryInfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// 注册库存模块基础设施依赖。
    /// </summary>
    public static IServiceCollection AddInventoryInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<InventoryDbContext>());

        return services;
    }
}
