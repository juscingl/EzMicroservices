using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Orders.Infrastructure.EntityFrameworkCore.DbContexts;

/// <summary>
/// 设计时 DbContext 工厂，供 EF Core 迁移命令创建上下文实例。
/// </summary>
public sealed class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    /// <summary>
    /// 创建设计时 OrdersDbContext。
    /// </summary>
    public OrdersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
        optionsBuilder.UseNpgsql(GetConnectionString("ORDERS_DB_CONNECTION_STRING", "ordersdb"));
        return new OrdersDbContext(optionsBuilder.Options, NullCurrentUserAccessor.Instance);
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
