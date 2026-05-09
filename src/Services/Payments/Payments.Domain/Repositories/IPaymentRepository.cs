using BuildingBlocks.Repositories;
using Payments.Domain.Entities;

namespace Payments.Domain.Repositories;

/// <summary>
/// 支付仓储接口。
/// </summary>
public interface IPaymentRepository : IRepository<Payment, Guid>
{
    /// <summary>
    /// 按订单标识查询支付记录。
    /// </summary>
    Task<Payment?> FindByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
