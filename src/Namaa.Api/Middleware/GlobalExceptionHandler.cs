using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Namaa.Api.Middleware;

// Only unexpected exceptions reach this boundary; normal business failures use Result instead.
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private static readonly Action<ILogger, string, Exception?> LogUnhandledException =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, "UnhandledException"),
            "Unhandled exception while processing {RequestPath}");

    private static readonly Action<ILogger, int, string, Exception?> LogExpectedException =
        LoggerMessage.Define<int, string>(
            LogLevel.Warning,
            new EventId(2, "ExpectedRequestFailure"),
            "Request failed with status {StatusCode} at {RequestPath}");

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ValidationException => (
                StatusCodes.Status400BadRequest,
                "Validation failed.",
                "One or more request values are invalid."),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized.",
                "Authentication is required to access this resource."),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found.",
                "The requested resource does not exist."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                "An internal server error occurred.")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            LogUnhandledException(logger, httpContext.Request.Path, exception);
        }
        else
        {
            LogExpectedException(logger, statusCode, httpContext.Request.Path, exception);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
