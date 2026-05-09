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

/// <summary>
/// 订单应用服务实现，负责下单、查询与搜索。
/// </summary>
public sealed class OrderService(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher integrationEventPublisher,
    IOrderSearchIndexer orderSearchIndexer,
    IOrderSearchReader orderSearchReader,
    ILogger<OrderService> logger) : IOrderService
{
    /// <summary>
    /// 创建订单并发布集成事件与搜索索引。
    /// </summary>
    /// <param name="command">下单命令。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>订单标识。</returns>
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

    /// <summary>
    /// 根据标识获取订单详情。
    /// </summary>
    /// <param name="id">订单标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>订单实体；不存在时返回空。</returns>
    public Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return orderRepository.FindWithDetailsAsync(id, cancellationToken);
    }

    /// <summary>
    /// 查询订单搜索结果。
    /// </summary>
    /// <param name="keyword">搜索关键字。</param>
    /// <param name="customerId">客户标识。</param>
    /// <param name="size">返回数量上限。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>订单搜索结果集合。</returns>
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
