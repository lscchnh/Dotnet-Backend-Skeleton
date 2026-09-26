using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBackendSkeleton.Api.Infrastructure;

/// <summary>
/// Turns every unhandled exception into an RFC 9457 problem details response.
/// Map your own domain exceptions to status codes in <see cref="GetStatusCode"/>.
/// </summary>
public sealed partial class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = GetStatusCode(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            LogUnhandledException(logger, exception, httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                // Never leak internal details outside of development.
                Detail = environment.IsDevelopment() ? exception.Message : null,
            },
        });
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        BadHttpRequestException badRequest => badRequest.StatusCode,
        _ => StatusCodes.Status500InternalServerError,
    };

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception while processing {Method} {Path}")]
    private static partial void LogUnhandledException(ILogger logger, Exception exception, string method, string path);
}
