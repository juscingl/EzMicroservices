using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Inventory.Infrastructure.EntityFrameworkCore.DbContexts;

public sealed class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
        optionsBuilder.UseNpgsql(GetConnectionString("INVENTORY_DB_CONNECTION_STRING", "inventorydb"));
        return new InventoryDbContext(optionsBuilder.Options, NullCurrentUserAccessor.Instance);
    }

    private static string GetConnectionString(string environmentVariableName, string databaseName)
    {
        return Environment.GetEnvironmentVariable(environmentVariableName)
            ?? $"Host=localhost;Port=5432;Database={databaseName};Username=postgres;Password=postgres";
    }
}
