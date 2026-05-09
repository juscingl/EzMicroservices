namespace Orders.Application.Search;

/// <summary>
/// 订单搜索结果。
/// </summary>
/// <param name="OrderId">订单标识。</param>
/// <param name="CustomerId">客户标识。</param>
/// <param name="TotalAmount">订单总金额。</param>
/// <param name="Currency">币种。</param>
/// <param name="ItemCount">商品项数量。</param>
/// <param name="IndexedAtUtc">索引时间（UTC）。</param>
/// <param name="Lines">订单行结果集合。</param>
public sealed record OrderSearchResult(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    string Currency,
    int ItemCount,
    DateTime IndexedAtUtc,
    IReadOnlyCollection<OrderSearchLineResult> Lines);

/// <summary>
/// 订单搜索结果中的行项目。
/// </summary>
/// <param name="ProductId">商品标识。</param>
/// <param name="Quantity">数量。</param>
/// <param name="UnitPrice">单价。</param>
public sealed record OrderSearchLineResult(Guid ProductId, int Quantity, decimal UnitPrice);
