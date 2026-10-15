using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class ProjectCorrespondenceErrors
{
    public static readonly Error NotFound = new("ProjectCorrespondences.NotFound", "The requested project correspondence was not found.", ErrorType.NotFound);
    public static readonly Error ProjectNotFound = new("ProjectCorrespondences.ProjectNotFound", "The project was not found.", ErrorType.NotFound);
    public static readonly Error UserNotFound = new("ProjectCorrespondences.UserNotFound", "The creating user was not found.", ErrorType.NotFound);
}

