using BuildingBlocks.Auditing;
using BuildingBlocks.EntityFrameworkCore.Persistence;
using Orders.Domain.Entities;
using Orders.Infrastructure.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Orders.Infrastructure.EntityFrameworkCore.DbContexts;

/// <summary>
/// 订单模块数据库上下文。
/// </summary>
public sealed class OrdersDbContext(
    DbContextOptions<OrdersDbContext> options,
    ICurrentUserAccessor currentUserAccessor)
    : PlatformDbContext<OrdersDbContext>(options, currentUserAccessor)
{
    /// <summary>
    /// 订单数据集。
    /// </summary>
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
