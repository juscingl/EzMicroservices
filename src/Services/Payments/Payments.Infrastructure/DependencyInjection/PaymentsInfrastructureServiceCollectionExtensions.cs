using BuildingBlocks.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Domain.Repositories;
using Payments.Infrastructure.EntityFrameworkCore.DbContexts;
using Payments.Infrastructure.Repositories;

namespace Payments.Infrastructure.DependencyInjection;

public static class PaymentsInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<PaymentsDbContext>());

        return services;
    }
}
