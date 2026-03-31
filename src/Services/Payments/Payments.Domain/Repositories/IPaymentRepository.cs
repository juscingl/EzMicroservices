using BuildingBlocks.Repositories;
using Payments.Domain.Entities;

namespace Payments.Domain.Repositories;

public interface IPaymentRepository : IRepository<Payment, Guid>
{
    Task<Payment?> FindByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
