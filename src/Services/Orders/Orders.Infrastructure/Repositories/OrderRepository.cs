using BuildingBlocks.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;
using Orders.Infrastructure.EntityFrameworkCore.DbContexts;

namespace Orders.Infrastructure.Repositories;

public sealed class OrderRepository(OrdersDbContext dbContext)
    : Repository<OrdersDbContext, Order, Guid>(dbContext), IOrderRepository
{
    public Task<Order?> FindWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbContext.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public override Task<Order?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return FindWithDetailsAsync(id, cancellationToken);
    }
}
