using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.EntityFrameworkCore.Configurations;

public sealed class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
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
