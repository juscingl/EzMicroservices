using BuildingBlocks.Auditing;
using BuildingBlocks.EntityFrameworkCore.Persistence;
using Orders.Domain.Entities;
using Orders.Infrastructure.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Orders.Infrastructure.EntityFrameworkCore.DbContexts;

public sealed class OrdersDbContext(
    DbContextOptions<OrdersDbContext> options,
    ICurrentUserAccessor currentUserAccessor)
    : PlatformDbContext<OrdersDbContext>(options, currentUserAccessor)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
