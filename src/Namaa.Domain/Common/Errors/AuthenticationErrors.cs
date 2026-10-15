using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials = new(
        "Authentication.InvalidCredentials",
        "The email or password is invalid.",
        ErrorType.Unauthorized);

    public static readonly Error InvalidRefreshToken = new(
        "Authentication.InvalidRefreshToken",
        "The refresh token is invalid.",
        ErrorType.Unauthorized);

    public static readonly Error ExpiredRefreshToken = new(
        "Authentication.ExpiredRefreshToken",
        "The refresh token has expired.",
        ErrorType.Unauthorized);

    public static readonly Error RevokedRefreshToken = new(
        "Authentication.RevokedRefreshToken",
        "The refresh token has been revoked.",
        ErrorType.Unauthorized);

    public static readonly Error UserInactive = new(
        "Authentication.UserInactive",
        "The user account is inactive.",
        ErrorType.Unauthorized);
}
