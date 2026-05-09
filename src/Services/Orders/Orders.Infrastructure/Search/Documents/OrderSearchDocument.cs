using Orders.Application.Search;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Search.Documents;

/// <summary>
/// 订单搜索文档模型，对应 Elasticsearch 中的订单索引结构。
/// </summary>
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

    /// <summary>
    /// 将订单聚合转换为可写入索引的文档。
    /// </summary>
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

    /// <summary>
    /// 将索引文档转换为应用层搜索结果对象。
    /// </summary>
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

/// <summary>
/// 订单行搜索文档模型。
/// </summary>
internal sealed record OrderSearchLineDocument(string ProductId, int Quantity, decimal UnitPrice);
