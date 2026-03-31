namespace BuildingBlocks.Security.Authorization;

public static class PlatformAuthorizationPolicies
{
    public const string AuthenticatedUser = "platform.authenticated";

    public const string OrdersRead = "platform.permissions.orders.read";
    public const string OrdersWrite = "platform.permissions.orders.write";
    public const string InventoryRead = "platform.permissions.inventory.read";
    public const string InventoryWrite = "platform.permissions.inventory.write";
    public const string PaymentsRead = "platform.permissions.payments.read";
    public const string PaymentsWrite = "platform.permissions.payments.write";
    public const string IdentityManage = "platform.permissions.identity.manage";

    public const string UsersRead = "platform.permissions.identity.users.read";
    public const string UsersWrite = "platform.permissions.identity.users.write";
    public const string RolesRead = "platform.permissions.identity.roles.read";
    public const string RolesWrite = "platform.permissions.identity.roles.write";
    public const string MenusRead = "platform.permissions.identity.menus.read";
    public const string MenusWrite = "platform.permissions.identity.menus.write";
    public const string PermissionsRead = "platform.permissions.identity.permissions.read";
    public const string PermissionsWrite = "platform.permissions.identity.permissions.write";
}
