using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class ProjectStageErrors
{
    public static readonly Error NotFound = new(
        "ProjectStages.NotFound",
        "The requested project stage was not found.",
        ErrorType.NotFound);

    public static readonly Error ProjectNotFound = new(
        "ProjectStages.ProjectNotFound",
        "The project specified for the stage was not found.",
        ErrorType.NotFound);
}
