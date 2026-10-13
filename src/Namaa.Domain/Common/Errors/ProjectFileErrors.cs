using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class ProjectFileErrors
{
    public static readonly Error NotFound = new(
        "ProjectFiles.NotFound",
        "The requested project file was not found.",
        ErrorType.NotFound);

    public static readonly Error ProjectNotFound = new(
        "ProjectFiles.ProjectNotFound",
        "The project specified for the file was not found.",
        ErrorType.NotFound);

    public static readonly Error UploadedByUserNotFound = new(
        "ProjectFiles.UploadedByUserNotFound",
        "The user uploading the project file was not found.",
        ErrorType.NotFound);

    public static readonly Error DuplicateStorageKey = new(
        "ProjectFiles.DuplicateStorageKey",
        "A project file with the same storage key already exists.",
        ErrorType.Conflict);
}
