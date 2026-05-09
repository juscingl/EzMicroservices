using BuildingBlocks.Auditing;
using BuildingBlocks.EntityFrameworkCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Infrastructure.EntityFrameworkCore.Configurations;

namespace Payments.Infrastructure.EntityFrameworkCore.DbContexts;

/// <summary>
/// 支付模块数据库上下文。
/// </summary>
public sealed class PaymentsDbContext(
    DbContextOptions<PaymentsDbContext> options,
    ICurrentUserAccessor currentUserAccessor)
    : PlatformDbContext<PaymentsDbContext>(options, currentUserAccessor)
{
    /// <summary>
    /// 支付数据集。
    /// </summary>
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
    }
}
