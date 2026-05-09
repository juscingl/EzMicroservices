using BuildingBlocks.Auditing;
using BuildingBlocks.EntityFrameworkCore.Persistence;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.EntityFrameworkCore.DbContexts;

/// <summary>
/// 库存模块数据库上下文。
/// </summary>
public sealed class InventoryDbContext(
    DbContextOptions<InventoryDbContext> options,
    ICurrentUserAccessor currentUserAccessor)
    : PlatformDbContext<InventoryDbContext>(options, currentUserAccessor)
{
    /// <summary>
    /// 库存数据集。
    /// </summary>
    public DbSet<StockItem> StockItems => Set<StockItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new StockItemConfiguration());
    }
}
