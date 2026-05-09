using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.EntityFrameworkCore.Configurations;

/// <summary>
/// 订单实体映射配置。
/// </summary>
public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    /// <summary>
    /// 配置订单实体及其子集合映射。
    /// </summary>
    /// <param name="builder">实体构建器。</param>
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(order => order.Id);

        builder.Property(order => order.CustomerId).IsRequired();
        builder.Ignore(order => order.DomainEvents);
        builder.Ignore(order => order.Total);
        builder.Navigation(order => order.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(order => order.Items, items =>
        {
            items.ToTable("order_items");
            items.WithOwner().HasForeignKey("OrderId");
            items.HasKey(item => item.Id);
            items.Property(item => item.ProductId).IsRequired();
            items.Property(item => item.Quantity).IsRequired();
            items.Property(item => item.UnitPrice).HasColumnType("numeric(18,2)");
        });
    }
}
