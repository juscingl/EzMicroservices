namespace BuildingBlocks.Contracts.IntegrationEvents;

/// <summary>
/// 订单创建完成事件，用于通知下游（如支付、风控、履约）启动后续流程。
/// </summary>
public sealed record OrderCreatedIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// 订单标识。
    /// </summary>
    public required Guid OrderId { get; init; }

    /// <summary>
    /// 下单客户标识。
    /// </summary>
    public required Guid CustomerId { get; init; }

    /// <summary>
    /// 订单总金额。
    /// </summary>
    public required decimal TotalAmount { get; init; }

    /// <summary>
    /// 币种，例如 CNY/USD。
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    /// 订单行明细快照，供下游按需消费。
    /// </summary>
    public required IReadOnlyCollection<OrderCreatedLine> Lines { get; init; }
}

/// <summary>
/// 订单行事件模型。
/// </summary>
public sealed record OrderCreatedLine(Guid ProductId, int Quantity, decimal UnitPrice);
