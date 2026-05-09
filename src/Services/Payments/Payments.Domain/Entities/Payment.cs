using BuildingBlocks.Domain;

namespace Payments.Domain.Entities;

/// <summary>
/// 支付聚合根。
/// </summary>
public sealed class Payment : FullAuditedAggregateRoot
{
    /// <summary>
    /// 关联订单标识。
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// 支付金额。
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// 币种，默认 CNY。
    /// </summary>
    public string Currency { get; private set; } = "CNY";

    /// <summary>
    /// 支付状态。
    /// </summary>
    public PaymentStatus Status { get; private set; }

    private Payment()
    {
    }

    /// <summary>
    /// 创建支付实体，初始状态为 Pending。
    /// </summary>
    public Payment(Guid orderId, decimal amount, string currency = "CNY")
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Currency = currency;
        Status = PaymentStatus.Pending;
    }

    /// <summary>
    /// 标记支付成功。
    /// </summary>
    public void MarkSucceeded()
    {
        Status = PaymentStatus.Succeeded;
    }

    /// <summary>
    /// 标记支付失败。
    /// </summary>
    public void MarkFailed()
    {
        Status = PaymentStatus.Failed;
    }
}
