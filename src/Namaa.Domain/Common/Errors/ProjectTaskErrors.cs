using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class ProjectTaskErrors
{
    public static readonly Error NotFound = new(
        "ProjectTasks.NotFound",
        "The requested project task was not found.",
        ErrorType.NotFound);

    public static readonly Error ProjectNotFound = new(
        "ProjectTasks.ProjectNotFound",
        "The project specified for the task was not found.",
        ErrorType.NotFound);

    public static readonly Error StageNotFound = new(
        "ProjectTasks.StageNotFound",
        "The project stage specified for the task was not found.",
        ErrorType.NotFound);

    public static readonly Error InvalidStageForProject = new(
        "ProjectTasks.InvalidStageForProject",
        "The project stage specified for the task does not belong to the project.",
        ErrorType.Validation);

    public static readonly Error CreatedByUserNotFound = new(
        "ProjectTasks.CreatedByUserNotFound",
        "The user creating the task was not found.",
        ErrorType.NotFound);

    public static readonly Error AssignedUserNotFound = new(
        "ProjectTasks.AssignedUserNotFound",
        "The user assigned to the task was not found.",
        ErrorType.NotFound);

    public static readonly Error ParentTaskNotFound = new(
        "ProjectTasks.ParentTaskNotFound",
        "The parent task was not found in the specified project.",
        ErrorType.NotFound);
}
