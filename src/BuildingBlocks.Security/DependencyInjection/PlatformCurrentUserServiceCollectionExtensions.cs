using BuildingBlocks.Auditing;
using BuildingBlocks.Security.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Security.DependencyInjection;

/// <summary>
/// 当前用户访问器注册扩展。
/// </summary>
public static class PlatformCurrentUserServiceCollectionExtensions
{
    /// <summary>
    /// 注册当前用户访问器实现。
    /// </summary>
    public static IServiceCollection AddPlatformCurrentUserAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
        return services;
    }
}
