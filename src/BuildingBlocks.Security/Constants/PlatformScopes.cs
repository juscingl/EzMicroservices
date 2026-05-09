namespace BuildingBlocks.Security.Constants;

/// <summary>
/// 表示PlatformScopes。
/// </summary>
public static class PlatformScopes
{
    /// <summary>
    /// 表示Orders。
    /// </summary>
    public const string Orders = "orders";
    /// <summary>
    /// 表示Inventory。
    /// </summary>
    public const string Inventory = "inventory";
    /// <summary>
    /// 表示Payments。
    /// </summary>
    public const string Payments = "payments";
    /// <summary>
    /// 表示Identity。
    /// </summary>
    public const string Identity = "identity";

    /// <summary>
    /// 表示All。
    /// </summary>
    public static readonly IReadOnlyCollection<string> All =
    [
        Orders,
        Inventory,
        Payments,
        Identity
    ];
}
