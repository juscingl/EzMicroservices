using BuildingBlocks.Security.Constants;

namespace AuthCenter.Api.Services;

public static class PlatformAuthorizationSeedCatalog
{
    public static readonly IReadOnlyCollection<SeedMenu> Menus =
    [
        new("dashboard", "Dashboard", null, "/dashboard", "layout-dashboard", "DashboardPage", 10, true, true, "Platform dashboard"),
        new("orders", "Orders", null, "/orders", "shopping-cart", "OrdersPage", 20, true, true, "Order management"),
        new("inventory", "Inventory", null, "/inventory", "package", "InventoryPage", 30, true, true, "Inventory management"),
        new("payments", "Payments", null, "/payments", "wallet", "PaymentsPage", 40, true, true, "Payment management"),
        new("security", "Security", null, "/system/security", "shield", "SecurityLayout", 50, true, true, "Identity and access control"),
        new("security.users", "Users", "security", "/system/security/users", "users", "UsersPage", 10, true, true, "User management"),
        new("security.roles", "Roles", "security", "/system/security/roles", "key", "RolesPage", 20, true, true, "Role management"),
        new("security.menus", "Menus", "security", "/system/security/menus", "menu", "MenusPage", 30, true, true, "Menu management"),
        new("security.permissions", "Permissions", "security", "/system/security/permissions", "lock", "PermissionsPage", 40, true, true, "Permission management")
    ];

    public static readonly IReadOnlyCollection<SeedPermission> Permissions = BuildPermissions();

    private static IReadOnlyCollection<SeedPermission> BuildPermissions()
    {
        var permissions = new List<SeedPermission>
        {
            Permission(PlatformPermissions.DashboardView, "Dashboard.View", "dashboard", "dashboard", "view", "page", 10, "View dashboard"),

            Permission(PlatformPermissions.OrdersMenu, "Orders.Menu", "orders", "orders", "menu", "menu", 10, "Access orders menu"),
            Permission(PlatformPermissions.OrdersRead, "Orders.Read", "orders", "orders", "read", "scope", 20, "Order read scope"),
            Permission(PlatformPermissions.OrdersWrite, "Orders.Write", "orders", "orders", "write", "scope", 30, "Order write scope"),
            Permission(PlatformPermissions.OrdersView, "Orders.View", "orders", "orders", "view", "page", 40, "View orders list"),
            Permission(PlatformPermissions.OrdersDetail, "Orders.Detail", "orders", "orders", "detail", "page", 50, "View order detail"),
            Permission(PlatformPermissions.OrdersCreate, "Orders.Create", "orders", "orders", "create", "action", 60, "Create order"),
            Permission(PlatformPermissions.OrdersEdit, "Orders.Edit", "orders", "orders", "edit", "action", 70, "Edit order"),
            Permission(PlatformPermissions.OrdersDelete, "Orders.Delete", "orders", "orders", "delete", "action", 80, "Delete order"),
            Permission(PlatformPermissions.OrdersImport, "Orders.Import", "orders", "orders", "import", "action", 90, "Import orders"),
            Permission(PlatformPermissions.OrdersExport, "Orders.Export", "orders", "orders", "export", "action", 100, "Export orders"),
            Permission(PlatformPermissions.OrdersSubmitButton, "Orders.Button.Submit", "orders", "orders", "submit", "button", 110, "Submit order button"),
            Permission(PlatformPermissions.OrdersApproveButton, "Orders.Button.Approve", "orders", "orders", "approve", "button", 120, "Approve order button"),

            Permission(PlatformPermissions.InventoryMenu, "Inventory.Menu", "inventory", "inventory", "menu", "menu", 10, "Access inventory menu"),
            Permission(PlatformPermissions.InventoryRead, "Inventory.Read", "inventory", "inventory", "read", "scope", 20, "Inventory read scope"),
            Permission(PlatformPermissions.InventoryWrite, "Inventory.Write", "inventory", "inventory", "write", "scope", 30, "Inventory write scope"),
            Permission(PlatformPermissions.InventoryView, "Inventory.View", "inventory", "inventory", "view", "page", 40, "View inventory list"),
            Permission(PlatformPermissions.InventoryDetail, "Inventory.Detail", "inventory", "inventory", "detail", "page", 50, "View inventory detail"),
            Permission(PlatformPermissions.InventoryCreate, "Inventory.Create", "inventory", "inventory", "create", "action", 60, "Create inventory item"),
            Permission(PlatformPermissions.InventoryEdit, "Inventory.Edit", "inventory", "inventory", "edit", "action", 70, "Edit inventory item"),
            Permission(PlatformPermissions.InventoryDelete, "Inventory.Delete", "inventory", "inventory", "delete", "action", 80, "Delete inventory item"),
            Permission(PlatformPermissions.InventoryImport, "Inventory.Import", "inventory", "inventory", "import", "action", 90, "Import inventory"),
            Permission(PlatformPermissions.InventoryExport, "Inventory.Export", "inventory", "inventory", "export", "action", 100, "Export inventory"),
            Permission(PlatformPermissions.InventoryAdjustButton, "Inventory.Button.Adjust", "inventory", "inventory", "adjust", "button", 110, "Adjust inventory button"),

            Permission(PlatformPermissions.PaymentsMenu, "Payments.Menu", "payments", "payments", "menu", "menu", 10, "Access payments menu"),
            Permission(PlatformPermissions.PaymentsRead, "Payments.Read", "payments", "payments", "read", "scope", 20, "Payments read scope"),
            Permission(PlatformPermissions.PaymentsWrite, "Payments.Write", "payments", "payments", "write", "scope", 30, "Payments write scope"),
            Permission(PlatformPermissions.PaymentsView, "Payments.View", "payments", "payments", "view", "page", 40, "View payments list"),
            Permission(PlatformPermissions.PaymentsDetail, "Payments.Detail", "payments", "payments", "detail", "page", 50, "View payment detail"),
            Permission(PlatformPermissions.PaymentsCreate, "Payments.Create", "payments", "payments", "create", "action", 60, "Create payment"),
            Permission(PlatformPermissions.PaymentsEdit, "Payments.Edit", "payments", "payments", "edit", "action", 70, "Edit payment"),
            Permission(PlatformPermissions.PaymentsRefund, "Payments.Refund", "payments", "payments", "refund", "action", 80, "Refund payment"),
            Permission(PlatformPermissions.PaymentsExport, "Payments.Export", "payments", "payments", "export", "action", 90, "Export payments"),
            Permission(PlatformPermissions.PaymentsCaptureButton, "Payments.Button.Capture", "payments", "payments", "capture", "button", 100, "Capture payment button"),

            Permission(PlatformPermissions.SecurityMenu, "Security.Menu", "security", "identity", "menu", "menu", 10, "Access security menu"),
            Permission(PlatformPermissions.IdentityManage, "Identity.Manage", "security", "identity", "manage", "scope", 20, "Identity manage scope"),
            Permission(PlatformPermissions.UsersMenu, "Users.Menu", "security.users", "users", "menu", "menu", 30, "Access users menu"),
            Permission(PlatformPermissions.UsersView, "Users.View", "security.users", "users", "view", "page", 40, "View users list"),
            Permission(PlatformPermissions.UsersCreate, "Users.Create", "security.users", "users", "create", "action", 50, "Create user"),
            Permission(PlatformPermissions.UsersEdit, "Users.Edit", "security.users", "users", "edit", "action", 60, "Edit user"),
            Permission(PlatformPermissions.UsersDelete, "Users.Delete", "security.users", "users", "delete", "action", 70, "Delete user"),
            Permission(PlatformPermissions.UsersImport, "Users.Import", "security.users", "users", "import", "action", 80, "Import users"),
            Permission(PlatformPermissions.UsersExport, "Users.Export", "security.users", "users", "export", "action", 90, "Export users"),
            Permission(PlatformPermissions.UsersAssignRoleButton, "Users.Button.AssignRole", "security.users", "users", "assign-role", "button", 100, "Assign role button"),
            Permission(PlatformPermissions.UsersAssignPermissionButton, "Users.Button.AssignPermission", "security.users", "users", "assign-permission", "button", 110, "Assign permission button"),
            Permission(PlatformPermissions.RolesMenu, "Roles.Menu", "security.roles", "roles", "menu", "menu", 120, "Access roles menu"),
            Permission(PlatformPermissions.RolesView, "Roles.View", "security.roles", "roles", "view", "page", 130, "View roles list"),
            Permission(PlatformPermissions.RolesCreate, "Roles.Create", "security.roles", "roles", "create", "action", 140, "Create role"),
            Permission(PlatformPermissions.RolesEdit, "Roles.Edit", "security.roles", "roles", "edit", "action", 150, "Edit role"),
            Permission(PlatformPermissions.RolesDelete, "Roles.Delete", "security.roles", "roles", "delete", "action", 160, "Delete role"),
            Permission(PlatformPermissions.RolesAssignPermissionButton, "Roles.Button.AssignPermission", "security.roles", "roles", "assign-permission", "button", 170, "Assign role permission button"),
            Permission(PlatformPermissions.MenusMenu, "Menus.Menu", "security.menus", "menus", "menu", "menu", 180, "Access menus menu"),
            Permission(PlatformPermissions.MenusView, "Menus.View", "security.menus", "menus", "view", "page", 190, "View menus list"),
            Permission(PlatformPermissions.MenusCreate, "Menus.Create", "security.menus", "menus", "create", "action", 200, "Create menu"),
            Permission(PlatformPermissions.MenusEdit, "Menus.Edit", "security.menus", "menus", "edit", "action", 210, "Edit menu"),
            Permission(PlatformPermissions.MenusDelete, "Menus.Delete", "security.menus", "menus", "delete", "action", 220, "Delete menu"),
            Permission(PlatformPermissions.PermissionsMenu, "Permissions.Menu", "security.permissions", "permissions", "menu", "menu", 230, "Access permissions menu"),
            Permission(PlatformPermissions.PermissionsView, "Permissions.View", "security.permissions", "permissions", "view", "page", 240, "View permission list"),
            Permission(PlatformPermissions.PermissionsCreate, "Permissions.Create", "security.permissions", "permissions", "create", "action", 250, "Create permission"),
            Permission(PlatformPermissions.PermissionsEdit, "Permissions.Edit", "security.permissions", "permissions", "edit", "action", 260, "Edit permission"),
            Permission(PlatformPermissions.PermissionsDelete, "Permissions.Delete", "security.permissions", "permissions", "delete", "action", 270, "Delete permission"),
            Permission(PlatformPermissions.PermissionsExport, "Permissions.Export", "security.permissions", "permissions", "export", "action", 280, "Export permissions")
        };

        return permissions;
    }

    private static SeedPermission Permission(
        string code,
        string name,
        string menuCode,
        string resource,
        string action,
        string permissionType,
        int sort,
        string description)
    {
        return new SeedPermission(code, name, menuCode, resource, action, permissionType, sort, true, true, description);
    }
}

public sealed record SeedMenu(
    string Code,
    string Name,
    string? ParentCode,
    string Route,
    string? Icon,
    string? Component,
    int Sort,
    bool IsVisible,
    bool IsEnabled,
    string? Description);

public sealed record SeedPermission(
    string Code,
    string Name,
    string MenuCode,
    string Resource,
    string Action,
    string PermissionType,
    int Sort,
    bool IsSystem,
    bool IsEnabled,
    string? Description);
