using System.Security.Claims;
using AuthCenter.Api.Identity;
using AuthCenter.Api.Services;
using BuildingBlocks.Security.Authorization;
using BuildingBlocks.Security.Constants;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthCenter.Api.Endpoints;

/// <summary>
/// OpenID Connect 相关端点映射（token、userinfo）。
/// </summary>
public static class ConnectEndpoints
{
    /// <summary>
    /// 映射 connect 分组端点。
    /// </summary>
    public static IEndpointRouteBuilder MapConnectEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/connect")
            .WithTags("connect");

        group.MapPost("/token", ExchangeAsync)
            .AllowAnonymous();

        group.MapMethods("/userinfo", [HttpMethods.Get, HttpMethods.Post], GetUserInfoAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.AuthenticatedUser);

        return endpoints;
    }

    /// <summary>
    /// token 交换入口，支持 password / refresh_token / client_credentials。
    /// </summary>
    private static async Task<IResult> ExchangeAsync(
        HttpContext httpContext,
        UserManager<ApplicationUser> userManager,
        IPermissionGrantResolver permissionGrantResolver,
        IOpenIddictPrincipalFactory principalFactory)
    {
        var request = httpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be resolved.");

        if (request.IsPasswordGrantType())
        {
            var user = await userManager.FindByNameAsync(request.Username ?? string.Empty)
                ?? await userManager.FindByEmailAsync(request.Username ?? string.Empty);

            if (user is null || string.IsNullOrWhiteSpace(request.Password) || !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return OpenIddictError(Errors.InvalidGrant, "The username/email or password is invalid.");
            }

            if (!await IsUserActiveAsync(userManager, user))
            {
                return OpenIddictError(Errors.InvalidGrant, "The user is not allowed to sign in.");
            }

            var roles = await userManager.GetRolesAsync(user);
            var permissions = await permissionGrantResolver.GetUserPermissionCodesAsync(user.Id, roles);
            var result = principalFactory.CreateForUser(user, roles, permissions, request.GetScopes());

            if (result.RejectedScopes.Count > 0)
            {
                return OpenIddictError(
                    Errors.InvalidScope,
                    $"The requested scopes are not allowed: {string.Join(", ", result.RejectedScopes)}.");
            }

            return Results.SignIn(result.Principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsRefreshTokenGrantType())
        {
            var authenticateResult = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
            {
                return OpenIddictError(Errors.InvalidGrant, "The refresh token is invalid.");
            }

            var subject = authenticateResult.Principal.FindFirstValue(Claims.Subject);
            if (string.IsNullOrWhiteSpace(subject))
            {
                return OpenIddictError(Errors.InvalidGrant, "The refresh token subject is missing.");
            }

            var user = await userManager.FindByIdAsync(subject);
            if (user is null || !await IsUserActiveAsync(userManager, user))
            {
                return OpenIddictError(Errors.InvalidGrant, "The refresh token is no longer valid for this user.");
            }

            var roles = await userManager.GetRolesAsync(user);
            var permissions = await permissionGrantResolver.GetUserPermissionCodesAsync(user.Id, roles);
            var scopes = request.GetScopes().Any() ? request.GetScopes() : authenticateResult.Principal.GetScopes();
            var result = principalFactory.CreateForUser(user, roles, permissions, scopes);

            if (result.RejectedScopes.Count > 0)
            {
                return OpenIddictError(
                    Errors.InvalidScope,
                    $"The requested scopes are not allowed: {string.Join(", ", result.RejectedScopes)}.");
            }

            return Results.SignIn(result.Principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsClientCredentialsGrantType())
        {
            OpenIddictPrincipalFactoryResult result;
            try
            {
                result = principalFactory.CreateForClient(request.ClientId ?? string.Empty, request.GetScopes());
            }
            catch (InvalidOperationException)
            {
                return OpenIddictError(Errors.InvalidClient, "The client is not registered.");
            }

            if (result.RejectedScopes.Count > 0)
            {
                return OpenIddictError(
                    Errors.InvalidScope,
                    $"The requested scopes are not allowed: {string.Join(", ", result.RejectedScopes)}.");
            }

            return Results.SignIn(result.Principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return OpenIddictError(Errors.UnsupportedGrantType, "The specified grant type is not supported.");
    }

    /// <summary>
    /// userinfo 端点，返回当前主体的基础信息。
    /// </summary>
    private static IResult GetUserInfoAsync(ClaimsPrincipal principal)
    {
        return Results.Ok(new
        {
            sub = principal.FindFirstValue(Claims.Subject) ?? string.Empty,
            name = principal.FindFirstValue(Claims.Name),
            email = principal.FindFirstValue(Claims.Email),
            roles = principal.FindAll(Claims.Role)
                .Select(claim => claim.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            permissions = principal.FindAll(PlatformClaimTypes.Permission)
                .Select(claim => claim.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray()
        });
    }

    private static async Task<bool> IsUserActiveAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user)
    {
        if (user.IsDeleted || !user.IsEnabled || await userManager.IsLockedOutAsync(user))
        {
            return false;
        }

        if (userManager.Options.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
        {
            return false;
        }

        return true;
    }

    private static IResult OpenIddictError(string error, string description)
    {
        return Results.Forbid(
            new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = error,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = description
            }),
            [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
    }
}
