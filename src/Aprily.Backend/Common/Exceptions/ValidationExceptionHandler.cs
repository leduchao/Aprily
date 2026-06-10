using Aprily.Backend.Common.Results;

using FluentValidation;

using Microsoft.AspNetCore.Diagnostics;

namespace Aprily.Backend.Common.Exceptions;

internal sealed class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<ValidationExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        var errorMessage = string.Join(
            "; ",
            validationException.Errors
                .Select(error => error.ErrorMessage)
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .Distinct());

        _logger.LogWarning(
            validationException,
            "Validation failed: {ValidationErrors}",
            errorMessage);

        var result = Result.Failure(
            new Error("validation.failed", errorMessage));

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

        return true;
    }
}
