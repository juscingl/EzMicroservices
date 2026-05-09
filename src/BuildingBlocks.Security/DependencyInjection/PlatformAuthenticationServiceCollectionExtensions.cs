using BuildingBlocks.Security.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;

namespace BuildingBlocks.Security.DependencyInjection;

/// <summary>
/// 认证注册扩展，统一接入 OpenIddict Validation 资源服务校验。
/// </summary>
public static class PlatformAuthenticationServiceCollectionExtensions
{
    /// <summary>
    /// 注册平台认证能力。
    /// </summary>
    public static IServiceCollection AddPlatformAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authenticationSection = configuration.GetSection(PlatformAuthenticationOptions.SectionName);
        services.Configure<PlatformAuthenticationOptions>(authenticationSection);

        var authenticationOptions =
            authenticationSection.Get<PlatformAuthenticationOptions>() ?? new PlatformAuthenticationOptions();

        // 认证后可在应用层直接读取当前用户信息。
        services.AddPlatformCurrentUserAccessor();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

        services.AddOpenIddict()
            .AddValidation(options =>
            {
                options.SetIssuer(new Uri(authenticationOptions.Authority));
                options.AddAudiences(authenticationOptions.Audience);
                options.UseSystemNetHttp();
                options.UseAspNetCore();
            });

        return services;
    }
}
