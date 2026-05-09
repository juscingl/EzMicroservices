using BuildingBlocks.Uow;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Application.Services;

/// <summary>
/// 支付应用服务实现，负责支付记录创建与状态更新。
/// </summary>
public sealed class PaymentService(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork) : IPaymentService
{
    /// <summary>
    /// 按订单执行支付：已存在记录则更新状态，不存在则新建记录。
    /// </summary>
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

    /// <summary>
    /// 按订单标识获取支付记录。
    /// </summary>
    public Task<Payment?> GetAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return paymentRepository.FindByOrderIdAsync(orderId, cancellationToken);
    }
}
