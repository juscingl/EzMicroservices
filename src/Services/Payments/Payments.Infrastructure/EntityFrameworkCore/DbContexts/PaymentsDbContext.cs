using BuildingBlocks.Auditing;
using BuildingBlocks.EntityFrameworkCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Infrastructure.EntityFrameworkCore.Configurations;

namespace Payments.Infrastructure.EntityFrameworkCore.DbContexts;

public sealed class PaymentsDbContext(
    DbContextOptions<PaymentsDbContext> options,
    ICurrentUserAccessor currentUserAccessor)
    : PlatformDbContext<PaymentsDbContext>(options, currentUserAccessor)
{
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
    }
}
