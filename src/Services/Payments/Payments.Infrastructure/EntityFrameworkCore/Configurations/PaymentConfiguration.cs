using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.EntityFrameworkCore.Configurations;

/// <summary>
/// 支付实体映射配置。
/// </summary>
public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    /// <summary>
    /// 配置支付实体映射规则。
    /// </summary>
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
