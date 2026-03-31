using BuildingBlocks.Uow;
using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Contracts.Messaging;
using BuildingBlocks.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using Orders.Application.Commands;
using Orders.Application.Search;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;

namespace Orders.Application.Services;

public sealed class OrderService(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher integrationEventPublisher,
    IOrderSearchIndexer orderSearchIndexer,
    IOrderSearchReader orderSearchReader,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<Guid> PlaceAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default)
    {
        var items = command.Lines.Select(line => new OrderItem(line.ProductId, line.Quantity, line.UnitPrice));
        var order = new Order(command.CustomerId, items);

        await orderRepository.InsertAsync(order, cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var integrationEvent = new OrderCreatedIntegrationEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.Total,
            Currency = "CNY",
            Lines = order.Items
                .Select(item => new OrderCreatedLine(item.ProductId, item.Quantity, item.UnitPrice))
                .ToArray()
        };

        await PublishOrderCreatedEventAsync(integrationEvent, cancellationToken);
        await IndexOrderAsync(order, cancellationToken);

        return order.Id;
    }

    public Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return orderRepository.FindWithDetailsAsync(id, cancellationToken);
    }

    public Task<IReadOnlyCollection<OrderSearchResult>> SearchAsync(
        string? keyword,
        Guid? customerId,
        int size = 20,
        CancellationToken cancellationToken = default)
    {
        return orderSearchReader.SearchAsync(keyword, customerId, size, cancellationToken);
    }

    private async Task PublishOrderCreatedEventAsync(
        OrderCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        try
        {
            await integrationEventPublisher.PublishAsync(
                integrationEvent,
                IntegrationRoutingKeys.OrdersCreated,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Order {OrderId} was persisted but publishing the integration event failed.",
                integrationEvent.OrderId);
        }
    }

    private async Task IndexOrderAsync(Order order, CancellationToken cancellationToken)
    {
        try
        {
            await orderSearchIndexer.IndexAsync(order, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Order {OrderId} was persisted but Elasticsearch indexing failed.",
                order.Id);
        }
    }
}
