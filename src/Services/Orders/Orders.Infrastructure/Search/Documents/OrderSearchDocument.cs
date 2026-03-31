using Orders.Application.Search;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Search.Documents;

internal sealed class OrderSearchDocument
{
    public required string OrderId { get; init; }

    public required string CustomerId { get; init; }

    public required decimal TotalAmount { get; init; }

    public required string Currency { get; init; }

    public required int ItemCount { get; init; }

    public required DateTime IndexedAtUtc { get; init; }

    public required IReadOnlyCollection<string> LineProductIds { get; init; }

    public required string SearchText { get; init; }

    public required IReadOnlyCollection<OrderSearchLineDocument> Lines { get; init; }

    public static OrderSearchDocument FromOrder(Order order)
    {
        var lines = order.Items
            .Select(item => new OrderSearchLineDocument(item.ProductId.ToString("D"), item.Quantity, item.UnitPrice))
            .ToArray();

        var productIds = lines.Select(line => line.ProductId).ToArray();

        return new OrderSearchDocument
        {
            OrderId = order.Id.ToString("D"),
            CustomerId = order.CustomerId.ToString("D"),
            TotalAmount = order.Total,
            Currency = "CNY",
            ItemCount = order.Items.Count,
            IndexedAtUtc = DateTime.UtcNow,
            LineProductIds = productIds,
            SearchText = string.Join(' ', new[] { order.Id.ToString("D"), order.CustomerId.ToString("D") }.Concat(productIds)),
            Lines = lines
        };
    }

    public OrderSearchResult ToSearchResult()
    {
        return new OrderSearchResult(
            Guid.Parse(OrderId),
            Guid.Parse(CustomerId),
            TotalAmount,
            Currency,
            ItemCount,
            IndexedAtUtc,
            Lines.Select(line => new OrderSearchLineResult(Guid.Parse(line.ProductId), line.Quantity, line.UnitPrice)).ToArray());
    }
}

internal sealed record OrderSearchLineDocument(string ProductId, int Quantity, decimal UnitPrice);
