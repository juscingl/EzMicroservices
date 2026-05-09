using Orders.Application.Dtos;

namespace Orders.Application.Commands;

/// <summary>
/// 下单命令，包含客户标识与订单行信息。
/// </summary>
/// <param name="CustomerId">客户标识。</param>
/// <param name="Lines">订单行集合。</param>
public sealed record PlaceOrderCommand(Guid CustomerId, IReadOnlyCollection<OrderLineDto> Lines);
