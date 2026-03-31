using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthCenter.Api.EntityFrameworkCore;

public sealed class AuthCenterDbContextFactory : IDesignTimeDbContextFactory<AuthCenterDbContext>
{
    public AuthCenterDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthCenterDbContext>();
        optionsBuilder.UseNpgsql(GetConnectionString("AUTHCENTER_DB_CONNECTION_STRING", "authdb"));
        return new AuthCenterDbContext(optionsBuilder.Options, NullCurrentUserAccessor.Instance);
    }

    private static string GetConnectionString(string environmentVariableName, string databaseName)
    {
        return Environment.GetEnvironmentVariable(environmentVariableName)
            ?? $"Host=localhost;Port=5432;Database={databaseName};Username=postgres;Password=postgres";
    }
}
