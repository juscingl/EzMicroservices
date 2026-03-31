using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Models;
using Microsoft.Extensions.Logging;
using Payments.Application.Services;

namespace Payments.Application.IntegrationHandlers;

public sealed class OrderCreatedPaymentHandler(
    IPaymentService paymentService,
    ILogger<OrderCreatedPaymentHandler> logger) : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
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
