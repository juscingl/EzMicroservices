using Payments.Domain.Entities;

namespace Payments.Application.Services;

public interface IPaymentService
{
    Task<Payment> CaptureAsync(
        Guid orderId,
        decimal amount,
        string currency = "CNY",
        CancellationToken cancellationToken = default);

    Task<Payment?> GetAsync(Guid orderId, CancellationToken cancellationToken = default);
}
