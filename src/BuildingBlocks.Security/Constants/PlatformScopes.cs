namespace BuildingBlocks.Security.Constants;

public static class PlatformScopes
{
    public const string Orders = "orders";
    public const string Inventory = "inventory";
    public const string Payments = "payments";
    public const string Identity = "identity";

    public static readonly IReadOnlyCollection<string> All =
    [
        Orders,
        Inventory,
        Payments,
        Identity
    ];
}
