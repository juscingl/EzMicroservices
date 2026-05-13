using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Exceptions;

public sealed class PlatformGlobalExceptionHandler(
    ILogger<PlatformGlobalExceptionHandler> logger,
    IHostEnvironment hostEnvironment) : IExceptionHandler
{
    private const string ErrorCodeExtensionKey = "errorCode";
    private const string TraceIdExtensionKey = "traceId";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        var (statusCode, title, errorCode) = MapException(exception);

        logger.LogError(
            exception,
            "Unhandled exception [{ErrorCode}] on {Path}. TraceId: {TraceId}",
            errorCode,
            httpContext.Request.Path,
            traceId);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions[ErrorCodeExtensionKey] = errorCode;
        problemDetails.Extensions[TraceIdExtensionKey] = traceId;

        if (hostEnvironment.IsDevelopment())
        {
            problemDetails.Detail = exception.Message;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title, string ErrorCode) MapException(Exception exception)
    {
        return exception switch
        {
            EntityNotFoundException => (StatusCodes.Status404NotFound, "Resource not found", "resource_not_found"),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Bad request", "bad_request"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden", "forbidden"),
            _ => (StatusCodes.Status500InternalServerError, "Unhandled server error", "unhandled_server_error")
        };
    }
}
