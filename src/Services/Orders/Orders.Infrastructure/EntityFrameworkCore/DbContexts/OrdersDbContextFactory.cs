using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Orders.Infrastructure.EntityFrameworkCore.DbContexts;

public sealed class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
        optionsBuilder.UseNpgsql(GetConnectionString("ORDERS_DB_CONNECTION_STRING", "ordersdb"));
        return new OrdersDbContext(optionsBuilder.Options, NullCurrentUserAccessor.Instance);
    }

    private static string GetConnectionString(string environmentVariableName, string databaseName)
    {
        return Environment.GetEnvironmentVariable(environmentVariableName)
            ?? $"Host=localhost;Port=5432;Database={databaseName};Username=postgres;Password=postgres";
    }
}
