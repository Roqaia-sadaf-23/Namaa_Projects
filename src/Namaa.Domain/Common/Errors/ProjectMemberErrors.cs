using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class ProjectMemberErrors
{
    public static readonly Error NotFound = new(
        "ProjectMembers.NotFound",
        "The requested project member was not found.",
        ErrorType.NotFound);

    public static readonly Error ProjectNotFound = new(
        "ProjectMembers.ProjectNotFound",
        "The project specified for the member was not found.",
        ErrorType.NotFound);

    public static readonly Error UserNotFound = new(
        "ProjectMembers.UserNotFound",
        "The user specified for the project membership was not found.",
        ErrorType.NotFound);

    public static readonly Error DuplicateMember = new(
        "ProjectMembers.DuplicateMember",
        "The user is already a member of the project.",
        ErrorType.Conflict);

    public static readonly Error InvalidLeaveDate = new(
        "ProjectMembers.InvalidLeaveDate",
        "The leave date cannot be before the join date.",
        ErrorType.Validation);
}
