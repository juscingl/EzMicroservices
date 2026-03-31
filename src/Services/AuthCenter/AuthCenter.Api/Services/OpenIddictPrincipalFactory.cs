using System.Security.Claims;
using AuthCenter.Api.Identity;
using BuildingBlocks.Security.Constants;
using BuildingBlocks.Security.Options;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace AuthCenter.Api.Services;

public sealed class OpenIddictPrincipalFactory(
    IOptions<PlatformAuthenticationOptions> authenticationOptions,
    ISeededClientRegistry seededClientRegistry) : IOpenIddictPrincipalFactory
{
    private readonly PlatformAuthenticationOptions _authenticationOptions = authenticationOptions.Value;

    public OpenIddictPrincipalFactoryResult CreateForUser(
        ApplicationUser user,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        IEnumerable<string> requestedScopes)
    {
        var roleList = roles.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var permissionList = permissions.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var allowedScopes = PlatformScopeCatalog.StandardScopes
            .Concat(PlatformScopeCatalog.GetAllowedScopesForPermissions(permissionList))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var scopeResolution = ResolveScopes(requestedScopes, allowedScopes);
        var grantedPermissions = FilterPermissions(permissionList, scopeResolution.GrantedScopes);

        var identity = new ClaimsIdentity(authenticationType: "OpenIddict");
        identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString()));

        if (!string.IsNullOrWhiteSpace(user.UserName))
        {
            identity.AddClaim(new Claim(OpenIddictConstants.Claims.Name, user.UserName));
        }

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            identity.AddClaim(new Claim(OpenIddictConstants.Claims.Email, user.Email));
        }

        foreach (var role in roleList)
        {
            identity.AddClaim(new Claim(OpenIddictConstants.Claims.Role, role));
        }

        foreach (var permission in grantedPermissions)
        {
            identity.AddClaim(new Claim(PlatformClaimTypes.Permission, permission));
        }

        var principal = BuildPrincipal(identity, scopeResolution.GrantedScopes);
        return new OpenIddictPrincipalFactoryResult(principal, scopeResolution.GrantedScopes, scopeResolution.RejectedScopes);
    }

    public OpenIddictPrincipalFactoryResult CreateForClient(
        string clientId,
        IEnumerable<string> requestedScopes)
    {
        var client = seededClientRegistry.Find(clientId)
            ?? throw new InvalidOperationException($"The client '{clientId}' is not registered in the seed registry.");

        var scopeResolution = ResolveScopes(requestedScopes, client.AllowedScopes);
        var grantedPermissions = FilterPermissions(client.GrantedPermissions, scopeResolution.GrantedScopes);

        var identity = new ClaimsIdentity(authenticationType: "OpenIddict");
        identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, client.ClientId));
        identity.AddClaim(new Claim(OpenIddictConstants.Claims.ClientId, client.ClientId));
        identity.AddClaim(new Claim(OpenIddictConstants.Claims.Name, client.DisplayName));

        foreach (var permission in grantedPermissions)
        {
            identity.AddClaim(new Claim(PlatformClaimTypes.Permission, permission));
        }

        var principal = BuildPrincipal(identity, scopeResolution.GrantedScopes);
        return new OpenIddictPrincipalFactoryResult(principal, scopeResolution.GrantedScopes, scopeResolution.RejectedScopes);
    }

    private ClaimsPrincipal BuildPrincipal(ClaimsIdentity identity, IReadOnlyCollection<string> grantedScopes)
    {
        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(grantedScopes);
        principal.SetResources(_authenticationOptions.Audience);

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(OpenIddictConstants.Destinations.AccessToken);
        }

        return principal;
    }

    private static ScopeResolution ResolveScopes(IEnumerable<string> requestedScopes, IEnumerable<string> allowedScopes)
    {
        var allowed = new HashSet<string>(allowedScopes, StringComparer.OrdinalIgnoreCase);
        var requested = requestedScopes
            .Where(scope => !string.IsNullOrWhiteSpace(scope))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (requested.Length == 0)
        {
            return new ScopeResolution(allowed.ToArray(), Array.Empty<string>());
        }

        var granted = requested.Where(allowed.Contains).ToArray();
        var rejected = requested.Where(scope => !allowed.Contains(scope)).ToArray();
        return new ScopeResolution(granted, rejected);
    }

    private static IReadOnlyCollection<string> FilterPermissions(
        IEnumerable<string> permissions,
        IEnumerable<string> grantedScopes)
    {
        var platformScopes = grantedScopes
            .Where(PlatformScopeCatalog.IsPlatformScope)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (platformScopes.Length == 0)
        {
            return Array.Empty<string>();
        }

        var allowedPermissions = new HashSet<string>(
            PlatformScopeCatalog.GetPermissionsForScopes(platformScopes),
            StringComparer.OrdinalIgnoreCase);

        return permissions
            .Where(allowedPermissions.Contains)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private sealed record ScopeResolution(
        IReadOnlyCollection<string> GrantedScopes,
        IReadOnlyCollection<string> RejectedScopes);
}

public sealed record OpenIddictPrincipalFactoryResult(
    ClaimsPrincipal Principal,
    IReadOnlyCollection<string> GrantedScopes,
    IReadOnlyCollection<string> RejectedScopes);
