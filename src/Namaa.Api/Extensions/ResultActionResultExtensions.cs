using Microsoft.AspNetCore.Mvc;
using Namaa.Domain.Common.Results;

namespace Namaa.Api.Extensions;

public static class ResultActionResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller) =>
        result.IsSuccess ? controller.Ok() : ToFailureActionResult(result.Error, controller);

    public static IActionResult ToActionResult<TValue>(
        this Result<TValue> result,
        ControllerBase controller) =>
        result.IsSuccess
            ? controller.Ok(result.Value)
            : ToFailureActionResult(result.Error, controller);

    private static ObjectResult ToFailureActionResult(Error error, ControllerBase controller)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(error.Type),
            Detail = error.Message,
            Instance = controller.HttpContext.Request.Path
        };
        problemDetails.Extensions["code"] = error.Code;

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    private static string GetTitle(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => "Validation failed.",
        ErrorType.Unauthorized => "Unauthorized.",
        ErrorType.Forbidden => "Forbidden.",
        ErrorType.NotFound => "Resource not found.",
        ErrorType.Conflict => "Conflict.",
        _ => "An operation failed."
    };
}
