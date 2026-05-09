using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Payments.Infrastructure.EntityFrameworkCore.DbContexts;

/// <summary>
/// 设计时 DbContext 工厂，供 EF Core 迁移命令使用。
/// </summary>
public sealed class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    /// <summary>
    /// 创建设计时 PaymentsDbContext。
    /// </summary>
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
        optionsBuilder.UseNpgsql(GetConnectionString("PAYMENTS_DB_CONNECTION_STRING", "paymentsdb"));
        return new PaymentsDbContext(optionsBuilder.Options, NullCurrentUserAccessor.Instance);
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
