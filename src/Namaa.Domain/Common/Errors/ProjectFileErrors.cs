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

    public static readonly Error InvalidFile = new(
        "ProjectFiles.InvalidFile",
        "The uploaded file name or content type is invalid.",
        ErrorType.Validation);

    public static readonly Error FileTooLarge = new(
        "ProjectFiles.FileTooLarge",
        "The uploaded file exceeds the configured size limit.",
        ErrorType.Validation);

    public static readonly Error FileTypeBlocked = new(
        "ProjectFiles.FileTypeBlocked",
        "The uploaded file type is not permitted.",
        ErrorType.Validation);

    public static readonly Error PhysicalFileNotFound = new(
        "ProjectFiles.PhysicalFileNotFound",
        "The requested file content is unavailable.",
        ErrorType.NotFound);

    public static readonly Error StorageUnavailable = new(
        "ProjectFiles.StorageUnavailable",
        "The file storage service is temporarily unavailable.",
        ErrorType.Failure);
}
