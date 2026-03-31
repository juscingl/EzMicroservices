using AuthCenter.Api.Endpoints;

namespace AuthCenter.Api.DependencyInjection;

public static class AuthCenterApplicationBuilderExtensions
{
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
