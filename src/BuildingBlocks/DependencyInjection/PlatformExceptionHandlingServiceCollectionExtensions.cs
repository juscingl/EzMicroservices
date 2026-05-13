using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.DependencyInjection;

public static class PlatformExceptionHandlingServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformExceptionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<PlatformGlobalExceptionHandler>();
        return services;
    }
}
