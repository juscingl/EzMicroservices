using BuildingBlocks.Security.Constants;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthCenter.Api.Services;

public static class PlatformScopeCatalog
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyCollection<string>> ScopePermissions =
        new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase)
        {
            [PlatformScopes.Orders] = PlatformPermissions.OrdersModule,
            [PlatformScopes.Inventory] = PlatformPermissions.InventoryModule,
            [PlatformScopes.Payments] = PlatformPermissions.PaymentsModule,
            [PlatformScopes.Identity] = PlatformPermissions.IdentityModule
        };

    public static readonly IReadOnlyCollection<string> StandardScopes =
    [
        Scopes.OpenId,
        Scopes.Profile,
        Scopes.Email,
        Scopes.Roles,
        Scopes.OfflineAccess
    ];

    public static bool IsPlatformScope(string scope)
    {
        return ScopePermissions.ContainsKey(scope);
    }

    public static IReadOnlyCollection<string> GetAllowedScopesForPermissions(IEnumerable<string> permissions)
    {
        var availablePermissions = new HashSet<string>(permissions, StringComparer.OrdinalIgnoreCase);
        var scopes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var mapping in ScopePermissions)
        {
            if (mapping.Value.Any(availablePermissions.Contains))
            {
                scopes.Add(mapping.Key);
            }
        }

        return scopes.ToArray();
    }

    public static IReadOnlyCollection<string> GetPermissionsForScopes(IEnumerable<string> scopes)
    {
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var scope in scopes)
        {
            if (!ScopePermissions.TryGetValue(scope, out var mappedPermissions))
            {
                continue;
            }

            foreach (var permission in mappedPermissions)
            {
                permissions.Add(permission);
            }
        }

        return permissions.ToArray();
    }
}
