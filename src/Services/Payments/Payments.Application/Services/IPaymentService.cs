using Payments.Domain.Entities;

namespace Payments.Application.Services;

/// <summary>
/// 支付应用服务接口。
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// 执行支付扣款（或模拟扣款）并返回支付记录。
    /// </summary>
    Task<Payment> CaptureAsync(
        Guid orderId,
        decimal amount,
        string currency = "CNY",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按订单标识查询支付记录。
    /// </summary>
    Task<Payment?> GetAsync(Guid orderId, CancellationToken cancellationToken = default);
}
