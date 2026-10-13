namespace Namaa.Api.Contracts.ProjectFiles;

public sealed record CreateProjectFileRequest(
    long UploadedByUserId,
    string FileName,
    string StorageKey,
    long FileSizeBytes,
    string? ContentType,
    string? Description,
    DateTime? UploadedAt);
