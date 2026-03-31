using BuildingBlocks.Security.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;

namespace BuildingBlocks.Security.DependencyInjection;

public static class PlatformAuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authenticationSection = configuration.GetSection(PlatformAuthenticationOptions.SectionName);
        services.Configure<PlatformAuthenticationOptions>(authenticationSection);

        var authenticationOptions =
            authenticationSection.Get<PlatformAuthenticationOptions>() ?? new PlatformAuthenticationOptions();

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
