using BuildingBlocks.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;
using Orders.Infrastructure.EntityFrameworkCore.DbContexts;

namespace Orders.Infrastructure.Repositories;

/// <summary>
/// 订单仓储实现，补充订单明细的聚合查询能力。
/// </summary>
public sealed class OrderRepository(OrdersDbContext dbContext)
    : Repository<OrdersDbContext, Order, Guid>(dbContext), IOrderRepository
{
    /// <summary>
    /// 查询订单及其明细集合。
    /// </summary>
    public Task<Order?> FindWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbContext.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    /// <summary>
    /// 重写基础查询，默认返回带明细的订单对象。
    /// </summary>
    public override Task<Order?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return FindWithDetailsAsync(id, cancellationToken);
    }
}
