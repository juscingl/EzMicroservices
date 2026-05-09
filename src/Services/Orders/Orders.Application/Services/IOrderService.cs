using Orders.Application.Commands;
using Orders.Application.Search;
using Orders.Domain.Entities;

namespace Orders.Application.Services;

/// <summary>
/// 订单应用服务。
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// 创建订单。
    /// </summary>
    /// <param name="command">下单命令。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>订单标识。</returns>
    Task<Guid> PlaceAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据订单标识查询订单详情。
    /// </summary>
    /// <param name="id">订单标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>订单实体；不存在时返回空。</returns>
    Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按条件检索订单。
    /// </summary>
    /// <param name="keyword">搜索关键字。</param>
    /// <param name="customerId">客户标识。</param>
    /// <param name="size">返回数量上限。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>订单搜索结果集合。</returns>
    Task<IReadOnlyCollection<OrderSearchResult>> SearchAsync(
        string? keyword,
        Guid? customerId,
        int size = 20,
        CancellationToken cancellationToken = default);
}
