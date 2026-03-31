using BuildingBlocks.Auditing;
using BuildingBlocks.Security.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Security.DependencyInjection;

public static class PlatformCurrentUserServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformCurrentUserAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
        return services;
    }
}
