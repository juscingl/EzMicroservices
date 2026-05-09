using BuildingBlocks.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Domain.Repositories;
using Payments.Infrastructure.EntityFrameworkCore.DbContexts;
using Payments.Infrastructure.Repositories;

namespace Payments.Infrastructure.DependencyInjection;

/// <summary>
/// 支付模块基础设施依赖注入扩展。
/// </summary>
public static class PaymentsInfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// 注册支付模块基础设施依赖。
    /// </summary>
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<PaymentsDbContext>());

        return services;
    }
}
