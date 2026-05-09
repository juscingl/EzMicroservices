using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Inventory.Infrastructure.EntityFrameworkCore.DbContexts;

/// <summary>
/// 设计时 DbContext 工厂，供 EF Core 迁移命令使用。
/// </summary>
public sealed class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    /// <summary>
    /// 创建设计时 InventoryDbContext。
    /// </summary>
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
        optionsBuilder.UseNpgsql(GetConnectionString("INVENTORY_DB_CONNECTION_STRING", "inventorydb"));
        return new InventoryDbContext(optionsBuilder.Options, NullCurrentUserAccessor.Instance);
    }

    /// <summary>
    /// 读取连接串：优先环境变量，未配置时回退到本地默认值。
    /// </summary>
    private static string GetConnectionString(string environmentVariableName, string databaseName)
    {
        return Environment.GetEnvironmentVariable(environmentVariableName)
            ?? $"Host=localhost;Port=5432;Database={databaseName};Username=postgres;Password=postgres";
    }
}
