using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts;
using Namaa.Application.Common.Results;

namespace Namaa.Api.Extensions;

public static class ResultActionResultExtensions
{
    // Controllers stay thin: they return an application Result and this adapter selects the HTTP response.
    public static IActionResult ToActionResult(this Result result, ControllerBase controller) =>
        result.IsSuccess ? controller.Ok() : ToFailureActionResult(result.Error, controller);

    public static IActionResult ToActionResult<TValue>(
        this Result<TValue> result,
        ControllerBase controller) =>
        result.IsSuccess
            ? controller.Ok(result.Value)
            : ToFailureActionResult(result.Error, controller);

    private static IActionResult ToFailureActionResult(Error error, ControllerBase controller)
    {
        var response = new ApiErrorResponse(error.Code, error.Message, error.Type.ToString());

        return error.Type switch
        {
            ErrorType.Validation => controller.BadRequest(response),
            ErrorType.NotFound => controller.NotFound(response),
            ErrorType.Conflict => controller.Conflict(response),
            ErrorType.Unauthorized => controller.Unauthorized(response),
            ErrorType.Forbidden => controller.Forbid(),
            _ => controller.StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }
}
