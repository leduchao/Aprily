using Aprily.Backend.Common.Results;

using Microsoft.AspNetCore.Diagnostics;

namespace Aprily.Backend.Common.Exceptions;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, error) = exception switch
        {
            BadHttpRequestException => (
                StatusCodes.Status400BadRequest,
                new Error("request.invalid", exception.Message)),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                new Error("request.invalid", exception.Message)),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                new Error("auth.unauthorized", "Authentication is required to access this resource.")),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                new Error("resource.not_found", exception.Message)),
            NotImplementedException => (
                StatusCodes.Status501NotImplemented,
                new Error("server.not_implemented", "This operation is not implemented.")),
            TimeoutException => (
                StatusCodes.Status504GatewayTimeout,
                new Error("server.timeout", "The operation timed out.")),
            _ => (
                StatusCodes.Status500InternalServerError,
                new Error("server.error", "An unexpected server error occurred."))
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred. Status code: {StatusCode}",
                statusCode);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Request failed. Status code: {StatusCode}",
                statusCode);
        }

        var result = Result.Failure(error);

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

        return true;
    }
}
