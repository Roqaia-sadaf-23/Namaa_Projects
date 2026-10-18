using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class NotificationErrors
{
    public static readonly Error NotFound = new(
        "Notifications.NotFound",
        "The notification was not found.",
        ErrorType.NotFound);
}
