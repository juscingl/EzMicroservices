using AuthCenter.Api.EntityFrameworkCore;
using AuthCenter.Api.Services;

namespace AuthCenter.Api.HostedServices;

public sealed class AuthCenterInitializationHostedService(
    IServiceProvider serviceProvider,
    ILogger<AuthCenterInitializationHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AuthCenterDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var seeder = scope.ServiceProvider.GetRequiredService<IAuthCenterDataSeeder>();
        await seeder.SeedAsync(cancellationToken);

        logger.LogInformation("AuthCenter database initialization completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
