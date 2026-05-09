using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.EntityFrameworkCore.Configurations;

/// <summary>
/// 库存实体映射配置。
/// </summary>
public sealed class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    /// <summary>
    /// 配置库存实体映射规则。
    /// </summary>
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("stock_items");
        builder.HasKey(stockItem => stockItem.Id);

        builder.Property(stockItem => stockItem.SkuId).IsRequired();
        builder.Property(stockItem => stockItem.Quantity).IsRequired();
        builder.Ignore(stockItem => stockItem.DomainEvents);

        builder.HasIndex(stockItem => stockItem.SkuId).IsUnique();
    }
}
