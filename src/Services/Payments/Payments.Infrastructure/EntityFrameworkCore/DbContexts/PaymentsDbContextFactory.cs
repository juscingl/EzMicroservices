using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Payments.Infrastructure.EntityFrameworkCore.DbContexts;

public sealed class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
        optionsBuilder.UseNpgsql(GetConnectionString("PAYMENTS_DB_CONNECTION_STRING", "paymentsdb"));
        return new PaymentsDbContext(optionsBuilder.Options, NullCurrentUserAccessor.Instance);
    }

    private static string GetConnectionString(string environmentVariableName, string databaseName)
    {
        return Environment.GetEnvironmentVariable(environmentVariableName)
            ?? $"Host=localhost;Port=5432;Database={databaseName};Username=postgres;Password=postgres";
    }
}
