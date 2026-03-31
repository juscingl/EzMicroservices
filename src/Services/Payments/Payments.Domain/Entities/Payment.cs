using BuildingBlocks.Domain;

namespace Payments.Domain.Entities;

public sealed class Payment : FullAuditedAggregateRoot
{
    public Guid OrderId { get; private set; }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = "CNY";

    public PaymentStatus Status { get; private set; }

    private Payment()
    {
    }

    public Payment(Guid orderId, decimal amount, string currency = "CNY")
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Currency = currency;
        Status = PaymentStatus.Pending;
    }

    public void MarkSucceeded()
    {
        Status = PaymentStatus.Succeeded;
    }

    public void MarkFailed()
    {
        Status = PaymentStatus.Failed;
    }
}
