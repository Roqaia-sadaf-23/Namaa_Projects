using Namaa.Domain.Common.Results;

namespace Namaa.Application.Common.Errors;

public static class ApplicationErrors
{
    public static readonly Error Unauthenticated = new(
        "Application.Unauthenticated",
        "An authenticated user is required to perform this operation.",
        ErrorType.Unauthorized);
}
