using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.EntityFrameworkCore.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(payment => payment.Id);

        builder.Ignore(payment => payment.DomainEvents);
        builder.HasIndex(payment => payment.OrderId).IsUnique();
        builder.Property(payment => payment.Amount).HasColumnType("numeric(18,2)");
        builder.Property(payment => payment.Currency).HasMaxLength(8);
        builder.Property(payment => payment.Status).HasConversion<string>();
    }
}
