using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.EntityFrameworkCore.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
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
