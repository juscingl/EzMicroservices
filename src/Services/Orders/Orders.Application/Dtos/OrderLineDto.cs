namespace Orders.Application.Dtos;

/// <summary>
/// 订单行数据传输对象。
/// </summary>
/// <param name="ProductId">商品标识。</param>
/// <param name="Quantity">购买数量。</param>
/// <param name="UnitPrice">单价。</param>
public sealed record OrderLineDto(Guid ProductId, int Quantity, decimal UnitPrice);
