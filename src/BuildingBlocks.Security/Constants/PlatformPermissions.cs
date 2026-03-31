namespace BuildingBlocks.Security.Constants;

public static class PlatformPermissions
{
    public const string DashboardView = "dashboard.view";

    public const string OrdersMenu = "orders.menu";
    public const string OrdersRead = "orders.read";
    public const string OrdersWrite = "orders.write";
    public const string OrdersView = "orders.view";
    public const string OrdersDetail = "orders.detail";
    public const string OrdersCreate = "orders.create";
    public const string OrdersEdit = "orders.edit";
    public const string OrdersDelete = "orders.delete";
    public const string OrdersImport = "orders.import";
    public const string OrdersExport = "orders.export";
    public const string OrdersSubmitButton = "orders.button.submit";
    public const string OrdersApproveButton = "orders.button.approve";

    public const string InventoryMenu = "inventory.menu";
    public const string InventoryRead = "inventory.read";
    public const string InventoryWrite = "inventory.write";
    public const string InventoryView = "inventory.view";
    public const string InventoryDetail = "inventory.detail";
    public const string InventoryCreate = "inventory.create";
    public const string InventoryEdit = "inventory.edit";
    public const string InventoryDelete = "inventory.delete";
    public const string InventoryImport = "inventory.import";
    public const string InventoryExport = "inventory.export";
    public const string InventoryAdjustButton = "inventory.button.adjust";

    public const string PaymentsMenu = "payments.menu";
    public const string PaymentsRead = "payments.read";
    public const string PaymentsWrite = "payments.write";
    public const string PaymentsView = "payments.view";
    public const string PaymentsDetail = "payments.detail";
    public const string PaymentsCreate = "payments.create";
    public const string PaymentsEdit = "payments.edit";
    public const string PaymentsRefund = "payments.refund";
    public const string PaymentsExport = "payments.export";
    public const string PaymentsCaptureButton = "payments.button.capture";

    public const string SecurityMenu = "security.menu";
    public const string IdentityManage = "identity.manage";

    public const string UsersMenu = "identity.users.menu";
    public const string UsersView = "identity.users.view";
    public const string UsersCreate = "identity.users.create";
    public const string UsersEdit = "identity.users.edit";
    public const string UsersDelete = "identity.users.delete";
    public const string UsersImport = "identity.users.import";
    public const string UsersExport = "identity.users.export";
    public const string UsersAssignRoleButton = "identity.users.button.assign-role";
    public const string UsersAssignPermissionButton = "identity.users.button.assign-permission";

    public const string RolesMenu = "identity.roles.menu";
    public const string RolesView = "identity.roles.view";
    public const string RolesCreate = "identity.roles.create";
    public const string RolesEdit = "identity.roles.edit";
    public const string RolesDelete = "identity.roles.delete";
    public const string RolesAssignPermissionButton = "identity.roles.button.assign-permission";

    public const string MenusMenu = "identity.menus.menu";
    public const string MenusView = "identity.menus.view";
    public const string MenusCreate = "identity.menus.create";
    public const string MenusEdit = "identity.menus.edit";
    public const string MenusDelete = "identity.menus.delete";

    public const string PermissionsMenu = "identity.permissions.menu";
    public const string PermissionsView = "identity.permissions.view";
    public const string PermissionsCreate = "identity.permissions.create";
    public const string PermissionsEdit = "identity.permissions.edit";
    public const string PermissionsDelete = "identity.permissions.delete";
    public const string PermissionsExport = "identity.permissions.export";

    public static readonly IReadOnlyCollection<string> OrdersModule =
    [
        OrdersMenu,
        OrdersRead,
        OrdersWrite,
        OrdersView,
        OrdersDetail,
        OrdersCreate,
        OrdersEdit,
        OrdersDelete,
        OrdersImport,
        OrdersExport,
        OrdersSubmitButton,
        OrdersApproveButton
    ];

    public static readonly IReadOnlyCollection<string> InventoryModule =
    [
        InventoryMenu,
        InventoryRead,
        InventoryWrite,
        InventoryView,
        InventoryDetail,
        InventoryCreate,
        InventoryEdit,
        InventoryDelete,
        InventoryImport,
        InventoryExport,
        InventoryAdjustButton
    ];

    public static readonly IReadOnlyCollection<string> PaymentsModule =
    [
        PaymentsMenu,
        PaymentsRead,
        PaymentsWrite,
        PaymentsView,
        PaymentsDetail,
        PaymentsCreate,
        PaymentsEdit,
        PaymentsRefund,
        PaymentsExport,
        PaymentsCaptureButton
    ];

    public static readonly IReadOnlyCollection<string> IdentityModule =
    [
        SecurityMenu,
        IdentityManage,
        UsersMenu,
        UsersView,
        UsersCreate,
        UsersEdit,
        UsersDelete,
        UsersImport,
        UsersExport,
        UsersAssignRoleButton,
        UsersAssignPermissionButton,
        RolesMenu,
        RolesView,
        RolesCreate,
        RolesEdit,
        RolesDelete,
        RolesAssignPermissionButton,
        MenusMenu,
        MenusView,
        MenusCreate,
        MenusEdit,
        MenusDelete,
        PermissionsMenu,
        PermissionsView,
        PermissionsCreate,
        PermissionsEdit,
        PermissionsDelete,
        PermissionsExport
    ];

    public static readonly IReadOnlyCollection<string> All =
    [
        DashboardView,
        .. OrdersModule,
        .. InventoryModule,
        .. PaymentsModule,
        .. IdentityModule
    ];
}
