using BuildingBlocks.Repositories;
using Orders.Domain.Entities;

namespace Orders.Domain.Repositories;

/// <summary>
/// 订单仓储接口。
/// </summary>
public interface IOrderRepository : IRepository<Order, Guid>
{
    /// <summary>
    /// 查询包含明细项的订单。
    /// </summary>
    /// <param name="id">订单标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>订单实体；不存在时返回空。</returns>
    Task<Order?> FindWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
