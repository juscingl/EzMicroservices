using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthCenter.Api.EntityFrameworkCore;

/// <summary>
/// 设计时 DbContext 工厂，供 EF Core 迁移命令使用。
/// </summary>
public sealed class AuthCenterDbContextFactory : IDesignTimeDbContextFactory<AuthCenterDbContext>
{
    /// <summary>
    /// 创建设计时 AuthCenterDbContext。
    /// </summary>
    public AuthCenterDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthCenterDbContext>();
        optionsBuilder.UseNpgsql(GetConnectionString("AUTHCENTER_DB_CONNECTION_STRING", "authdb"));
        return new AuthCenterDbContext(optionsBuilder.Options, NullCurrentUserAccessor.Instance);
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
