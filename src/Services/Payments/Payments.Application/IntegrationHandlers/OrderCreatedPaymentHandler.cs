using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Models;
using Microsoft.Extensions.Logging;
using Payments.Application.Services;

namespace Payments.Application.IntegrationHandlers;

/// <summary>
/// 订单创建事件处理器：收到订单事件后触发支付处理。
/// </summary>
public sealed class OrderCreatedPaymentHandler(
    IPaymentService paymentService,
    ILogger<OrderCreatedPaymentHandler> logger) : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    /// <summary>
    /// 处理订单创建集成事件。
    /// </summary>
    public async Task HandleAsync(
        OrderCreatedIntegrationEvent integrationEvent,
        IntegrationEventContext context,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Handling OrderCreated integration event. EventId={EventId}, OrderId={OrderId}, CorrelationId={CorrelationId}",
            context.EventId,
            integrationEvent.OrderId,
            context.CorrelationId);

        await paymentService.CaptureAsync(
            integrationEvent.OrderId,
            integrationEvent.TotalAmount,
            integrationEvent.Currency,
            cancellationToken);
    }
}
