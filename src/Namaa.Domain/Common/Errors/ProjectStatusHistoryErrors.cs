using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class ProjectStatusHistoryErrors
{
    public static readonly Error NotFound = new("ProjectStatusHistory.NotFound", "The requested project status-history record was not found.", ErrorType.NotFound);
    public static readonly Error ProjectNotFound = new("ProjectStatusHistory.ProjectNotFound", "The project was not found.", ErrorType.NotFound);
    public static readonly Error UserNotFound = new("ProjectStatusHistory.UserNotFound", "The user changing the project status was not found.", ErrorType.NotFound);
}

