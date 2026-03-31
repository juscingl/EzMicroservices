using BuildingBlocks.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Payments.Infrastructure.EntityFrameworkCore.DbContexts;

namespace Payments.Infrastructure.Repositories;

public sealed class PaymentRepository(PaymentsDbContext dbContext)
    : Repository<PaymentsDbContext, Payment, Guid>(dbContext), IPaymentRepository
{
    public Task<Payment?> FindByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return DbContext.Payments.FirstOrDefaultAsync(payment => payment.OrderId == orderId, cancellationToken);
    }
}
