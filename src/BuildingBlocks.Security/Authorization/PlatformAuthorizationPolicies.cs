namespace BuildingBlocks.Security.Authorization;

/// <summary>
/// 表示PlatformAuthorizationPolicies。
/// </summary>
public static class PlatformAuthorizationPolicies
{
    /// <summary>
    /// 表示AuthenticatedUser。
    /// </summary>
    public const string AuthenticatedUser = "platform.authenticated";

    /// <summary>
    /// 表示OrdersRead。
    /// </summary>
    public const string OrdersRead = "platform.permissions.orders.read";
    /// <summary>
    /// 表示OrdersWrite。
    /// </summary>
    public const string OrdersWrite = "platform.permissions.orders.write";
    /// <summary>
    /// 表示InventoryRead。
    /// </summary>
    public const string InventoryRead = "platform.permissions.inventory.read";
    /// <summary>
    /// 表示InventoryWrite。
    /// </summary>
    public const string InventoryWrite = "platform.permissions.inventory.write";
    /// <summary>
    /// 表示PaymentsRead。
    /// </summary>
    public const string PaymentsRead = "platform.permissions.payments.read";
    /// <summary>
    /// 表示PaymentsWrite。
    /// </summary>
    public const string PaymentsWrite = "platform.permissions.payments.write";
    /// <summary>
    /// 表示IdentityManage。
    /// </summary>
    public const string IdentityManage = "platform.permissions.identity.manage";

    /// <summary>
    /// 表示UsersRead。
    /// </summary>
    public const string UsersRead = "platform.permissions.identity.users.read";
    /// <summary>
    /// 表示UsersWrite。
    /// </summary>
    public const string UsersWrite = "platform.permissions.identity.users.write";
    /// <summary>
    /// 表示RolesRead。
    /// </summary>
    public const string RolesRead = "platform.permissions.identity.roles.read";
    /// <summary>
    /// 表示RolesWrite。
    /// </summary>
    public const string RolesWrite = "platform.permissions.identity.roles.write";
    /// <summary>
    /// 表示MenusRead。
    /// </summary>
    public const string MenusRead = "platform.permissions.identity.menus.read";
    /// <summary>
    /// 表示MenusWrite。
    /// </summary>
    public const string MenusWrite = "platform.permissions.identity.menus.write";
    /// <summary>
    /// 表示PermissionsRead。
    /// </summary>
    public const string PermissionsRead = "platform.permissions.identity.permissions.read";
    /// <summary>
    /// 表示PermissionsWrite。
    /// </summary>
    public const string PermissionsWrite = "platform.permissions.identity.permissions.write";
}
