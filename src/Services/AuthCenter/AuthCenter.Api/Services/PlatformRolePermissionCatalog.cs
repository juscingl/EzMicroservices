using BuildingBlocks.Security.Constants;

namespace AuthCenter.Api.Services;

public static class PlatformRolePermissionCatalog
{
    public static readonly IReadOnlyDictionary<string, IReadOnlyCollection<string>> RolePermissions =
        new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase)
        {
            [PlatformRoles.Admin] = PlatformPermissions.All,
            [PlatformRoles.IdentityAdmin] = PlatformPermissions.IdentityModule,
            [PlatformRoles.Operator] =
            [
                PlatformPermissions.OrdersMenu,
                PlatformPermissions.OrdersRead,
                PlatformPermissions.OrdersWrite,
                PlatformPermissions.OrdersView,
                PlatformPermissions.OrdersDetail,
                PlatformPermissions.OrdersCreate,
                PlatformPermissions.OrdersEdit,
                PlatformPermissions.OrdersImport,
                PlatformPermissions.OrdersExport,
                PlatformPermissions.OrdersSubmitButton,
                PlatformPermissions.InventoryMenu,
                PlatformPermissions.InventoryRead,
                PlatformPermissions.InventoryWrite,
                PlatformPermissions.InventoryView,
                PlatformPermissions.InventoryDetail,
                PlatformPermissions.InventoryCreate,
                PlatformPermissions.InventoryEdit,
                PlatformPermissions.InventoryImport,
                PlatformPermissions.InventoryExport,
                PlatformPermissions.InventoryAdjustButton,
                PlatformPermissions.PaymentsMenu,
                PlatformPermissions.PaymentsRead,
                PlatformPermissions.PaymentsWrite,
                PlatformPermissions.PaymentsView,
                PlatformPermissions.PaymentsDetail,
                PlatformPermissions.PaymentsCreate,
                PlatformPermissions.PaymentsEdit,
                PlatformPermissions.PaymentsExport,
                PlatformPermissions.PaymentsCaptureButton
            ],
            [PlatformRoles.Viewer] =
            [
                PlatformPermissions.DashboardView,
                PlatformPermissions.OrdersMenu,
                PlatformPermissions.OrdersRead,
                PlatformPermissions.OrdersView,
                PlatformPermissions.OrdersDetail,
                PlatformPermissions.OrdersExport,
                PlatformPermissions.InventoryMenu,
                PlatformPermissions.InventoryRead,
                PlatformPermissions.InventoryView,
                PlatformPermissions.InventoryDetail,
                PlatformPermissions.InventoryExport,
                PlatformPermissions.PaymentsMenu,
                PlatformPermissions.PaymentsRead,
                PlatformPermissions.PaymentsView,
                PlatformPermissions.PaymentsDetail,
                PlatformPermissions.PaymentsExport
            ]
        };
}
