using AuthCenter.Api.EntityFrameworkCore;
using AuthCenter.Api.HostedServices;
using AuthCenter.Api.Identity;
using AuthCenter.Api.Options;
using AuthCenter.Api.Services;
using BuildingBlocks.Security.Constants;
using BuildingBlocks.Security.DependencyInjection;
using BuildingBlocks.Security.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthCenter.Api.DependencyInjection;

public static class AuthCenterServiceCollectionExtensions
{
    public static IServiceCollection AddAuthCenter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddProblemDetails();

        services.Configure<PlatformAuthenticationOptions>(
            configuration.GetSection(PlatformAuthenticationOptions.SectionName));
        services.Configure<AuthCenterSeedOptions>(
            configuration.GetSection(AuthCenterSeedOptions.SectionName));

        var authenticationOptions = configuration
            .GetSection(PlatformAuthenticationOptions.SectionName)
            .Get<PlatformAuthenticationOptions>() ?? new PlatformAuthenticationOptions();

        services.AddPlatformCurrentUserAccessor();

        services.AddDbContext<AuthCenterDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"));
            options.UseOpenIddict();
        });

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AuthCenterDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<AuthCenterDbContext>();
            })
            .AddServer(options =>
            {
                options.SetIssuer(new Uri(authenticationOptions.Issuer));
                options.SetTokenEndpointUris("/connect/token");
                options.SetUserInfoEndpointUris("/connect/userinfo");
                options.SetRevocationEndpointUris("/connect/revocation");

                options.AllowPasswordFlow();
                options.AllowRefreshTokenFlow();
                options.AllowClientCredentialsFlow();

                options.RegisterScopes(
                    Scopes.OpenId,
                    Scopes.Profile,
                    Scopes.Email,
                    Scopes.Roles,
                    Scopes.OfflineAccess,
                    PlatformScopes.Orders,
                    PlatformScopes.Inventory,
                    PlatformScopes.Payments,
                    PlatformScopes.Identity);

                options.SetAccessTokenLifetime(
                    TimeSpan.FromMinutes(authenticationOptions.AccessTokenExpirationMinutes));
                options.SetRefreshTokenLifetime(
                    TimeSpan.FromDays(authenticationOptions.RefreshTokenExpirationDays));

                options.DisableAccessTokenEncryption();
                options.AddDevelopmentEncryptionCertificate();
                options.AddDevelopmentSigningCertificate();

                var aspNetCore = options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableUserInfoEndpointPassthrough()
                    .EnableStatusCodePagesIntegration();

                if (authenticationOptions.Issuer.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    aspNetCore.DisableTransportSecurityRequirement();
                }
            })
            .AddValidation(options =>
            {
                options.SetIssuer(new Uri(authenticationOptions.Authority));
                options.AddAudiences(authenticationOptions.Audience);
                options.UseSystemNetHttp();
                options.UseAspNetCore();
            });

        services.AddPlatformAuthorization();
        services.AddHealthChecks().AddDbContextCheck<AuthCenterDbContext>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IPermissionGrantResolver, PermissionGrantResolver>();
        services.AddScoped<IAuthorizationManagementService, AuthorizationManagementService>();
        services.AddScoped<ISeededClientRegistry, SeededClientRegistry>();
        services.AddScoped<IOpenIddictPrincipalFactory, OpenIddictPrincipalFactory>();
        services.AddScoped<IAuthCenterDataSeeder, AuthCenterDataSeeder>();
        services.AddHostedService<AuthCenterInitializationHostedService>();

        return services;
    }
}
