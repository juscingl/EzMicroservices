namespace Orders.Application.Search;

public sealed record OrderSearchResult(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    string Currency,
    int ItemCount,
    DateTime IndexedAtUtc,
    IReadOnlyCollection<OrderSearchLineResult> Lines);

public sealed record OrderSearchLineResult(Guid ProductId, int Quantity, decimal UnitPrice);
