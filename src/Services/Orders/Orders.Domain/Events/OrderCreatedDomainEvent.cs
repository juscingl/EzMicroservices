using BuildingBlocks.Domain;

namespace Orders.Domain.Events;

/// <summary>
/// 订单创建领域事件。
/// </summary>
/// <param name="OrderId">订单标识。</param>
/// <param name="CustomerId">客户标识。</param>
/// <param name="Amount">订单金额。</param>
public sealed record OrderCreatedDomainEvent(Guid OrderId, Guid CustomerId, decimal Amount)
    : DomainEvent(DateTimeOffset.UtcNow);
