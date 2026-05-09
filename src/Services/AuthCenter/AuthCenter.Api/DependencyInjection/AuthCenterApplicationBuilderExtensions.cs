using AuthCenter.Api.Endpoints;

namespace AuthCenter.Api.DependencyInjection;

/// <summary>
/// AuthCenter 应用中间件扩展。
/// </summary>
public static class AuthCenterApplicationBuilderExtensions
{
    /// <summary>
    /// 挂载认证中心运行所需中间件和端点。
    /// </summary>
    public static WebApplication UseAuthCenter(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapAuthCenterEndpoints();
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/ready");

        return app;
    }
}
