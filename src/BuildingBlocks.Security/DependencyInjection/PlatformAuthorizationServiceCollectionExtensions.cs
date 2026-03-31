using BuildingBlocks.Security.Authorization;
using BuildingBlocks.Security.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Security.DependencyInjection;

public static class PlatformAuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformAuthorization(this IServiceCollection services)
    {
        var authorization = services.AddAuthorizationBuilder();

        authorization.AddPolicy(
            PlatformAuthorizationPolicies.AuthenticatedUser,
            policy => policy.RequireAuthenticatedUser());

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.OrdersRead,
            PlatformPermissions.OrdersRead,
            PlatformPermissions.OrdersView,
            PlatformPermissions.OrdersDetail,
            PlatformPermissions.OrdersExport);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.OrdersWrite,
            PlatformPermissions.OrdersWrite,
            PlatformPermissions.OrdersCreate,
            PlatformPermissions.OrdersEdit,
            PlatformPermissions.OrdersDelete,
            PlatformPermissions.OrdersImport,
            PlatformPermissions.OrdersSubmitButton,
            PlatformPermissions.OrdersApproveButton);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.InventoryRead,
            PlatformPermissions.InventoryRead,
            PlatformPermissions.InventoryView,
            PlatformPermissions.InventoryDetail,
            PlatformPermissions.InventoryExport);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.InventoryWrite,
            PlatformPermissions.InventoryWrite,
            PlatformPermissions.InventoryCreate,
            PlatformPermissions.InventoryEdit,
            PlatformPermissions.InventoryDelete,
            PlatformPermissions.InventoryImport,
            PlatformPermissions.InventoryAdjustButton);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.PaymentsRead,
            PlatformPermissions.PaymentsRead,
            PlatformPermissions.PaymentsView,
            PlatformPermissions.PaymentsDetail,
            PlatformPermissions.PaymentsExport);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.PaymentsWrite,
            PlatformPermissions.PaymentsWrite,
            PlatformPermissions.PaymentsCreate,
            PlatformPermissions.PaymentsEdit,
            PlatformPermissions.PaymentsRefund,
            PlatformPermissions.PaymentsCaptureButton);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.IdentityManage,
            new[] { PlatformPermissions.IdentityManage }.Concat(PlatformPermissions.IdentityModule));

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.UsersRead,
            PlatformPermissions.IdentityManage,
            PlatformPermissions.UsersView,
            PlatformPermissions.UsersExport);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.UsersWrite,
            PlatformPermissions.IdentityManage,
            PlatformPermissions.UsersCreate,
            PlatformPermissions.UsersEdit,
            PlatformPermissions.UsersDelete,
            PlatformPermissions.UsersImport,
            PlatformPermissions.UsersAssignRoleButton,
            PlatformPermissions.UsersAssignPermissionButton);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.RolesRead,
            PlatformPermissions.IdentityManage,
            PlatformPermissions.RolesView);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.RolesWrite,
            PlatformPermissions.IdentityManage,
            PlatformPermissions.RolesCreate,
            PlatformPermissions.RolesEdit,
            PlatformPermissions.RolesDelete,
            PlatformPermissions.RolesAssignPermissionButton);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.MenusRead,
            PlatformPermissions.IdentityManage,
            PlatformPermissions.MenusView);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.MenusWrite,
            PlatformPermissions.IdentityManage,
            PlatformPermissions.MenusCreate,
            PlatformPermissions.MenusEdit,
            PlatformPermissions.MenusDelete);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.PermissionsRead,
            PlatformPermissions.IdentityManage,
            PlatformPermissions.PermissionsView,
            PlatformPermissions.PermissionsExport);

        AddPermissionPolicy(
            authorization,
            PlatformAuthorizationPolicies.PermissionsWrite,
            PlatformPermissions.IdentityManage,
            PlatformPermissions.PermissionsCreate,
            PlatformPermissions.PermissionsEdit,
            PlatformPermissions.PermissionsDelete);

        return services;
    }

    private static void AddPermissionPolicy(
        AuthorizationBuilder authorizationBuilder,
        string policyName,
        IEnumerable<string> permissions)
    {
        var permissionArray = permissions
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        authorizationBuilder.AddPolicy(
            policyName,
            policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    if (permissionArray.Length == 0)
                    {
                        return false;
                    }

                    var grantedPermissions = context.User.FindAll(PlatformClaimTypes.Permission)
                        .Select(claim => claim.Value)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    return permissionArray.Any(grantedPermissions.Contains);
                });
            });
    }

    private static void AddPermissionPolicy(
        AuthorizationBuilder authorizationBuilder,
        string policyName,
        params string[] permissions)
    {
        AddPermissionPolicy(authorizationBuilder, policyName, permissions.AsEnumerable());
    }
}
