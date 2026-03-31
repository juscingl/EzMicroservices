using BuildingBlocks.Observability.Options;
using Elastic.CommonSchema.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BuildingBlocks.Observability.DependencyInjection;

public static class PlatformObservabilityExtensions
{
    public static WebApplicationBuilder AddPlatformObservability(this WebApplicationBuilder builder, string serviceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        var options = builder.Configuration
            .GetSection(ElasticLoggingOptions.SectionName)
            .Get<ElasticLoggingOptions>() ?? new ElasticLoggingOptions();

        if (!options.Enabled)
        {
            return builder;
        }

        var logDirectory = ResolveLogDirectory(builder.Environment.ContentRootPath, options.LogDirectory);
        Directory.CreateDirectory(logDirectory);

        var formatter = new EcsTextFormatter();
        var logFilePath = Path.Combine(logDirectory, $"{serviceName.ToLowerInvariant()}-.json");
        var minimumLevel = ParseLevel(options.MinimumLevel);
        var environmentName = builder.Environment.EnvironmentName;

        builder.Host.UseSerilog((context, _, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Is(minimumLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("service.name", serviceName)
                .Enrich.WithProperty("service.environment", environmentName)
                .WriteTo.File(
                    formatter,
                    logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: options.RetainedFileCountLimit,
                    shared: true);

            if (options.EnableConsoleSink)
            {
                loggerConfiguration.WriteTo.Console(formatter);
            }
        });

        return builder;
    }

    public static WebApplication UsePlatformObservability(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        return app;
    }

    private static string ResolveLogDirectory(string contentRootPath, string configuredDirectory)
    {
        if (string.IsNullOrWhiteSpace(configuredDirectory))
        {
            return Path.Combine(contentRootPath, "logs");
        }

        return Path.IsPathRooted(configuredDirectory)
            ? configuredDirectory
            : Path.Combine(contentRootPath, configuredDirectory);
    }

    private static LogEventLevel ParseLevel(string minimumLevel)
    {
        return Enum.TryParse<LogEventLevel>(minimumLevel, ignoreCase: true, out var parsed)
            ? parsed
            : LogEventLevel.Information;
    }
}
