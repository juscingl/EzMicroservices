using BuildingBlocks.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Payments.Infrastructure.EntityFrameworkCore.DbContexts;

namespace Payments.Infrastructure.Repositories;

/// <summary>
/// 支付仓储实现。
/// </summary>
public sealed class PaymentRepository(PaymentsDbContext dbContext)
    : Repository<PaymentsDbContext, Payment, Guid>(dbContext), IPaymentRepository
{
    /// <summary>
    /// 按订单标识查询支付记录。
    /// </summary>
    public Task<Payment?> FindByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return DbContext.Payments.FirstOrDefaultAsync(payment => payment.OrderId == orderId, cancellationToken);
    }
}
