using BuildingBlocks.Uow;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Application.Services;

public sealed class PaymentService(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork) : IPaymentService
{
    public async Task<Payment> CaptureAsync(
        Guid orderId,
        decimal amount,
        string currency = "CNY",
        CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.FindByOrderIdAsync(orderId, cancellationToken);
        if (payment is null)
        {
            payment = new Payment(orderId, amount, currency);
            payment.MarkSucceeded();
            await paymentRepository.InsertAsync(payment, cancellationToken: cancellationToken);
        }
        else
        {
            payment.MarkSucceeded();
            await paymentRepository.UpdateAsync(payment, cancellationToken: cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return payment;
    }

    public Task<Payment?> GetAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return paymentRepository.FindByOrderIdAsync(orderId, cancellationToken);
    }
}
