namespace Namaa.Api.Contracts.ProjectFiles;

public sealed record CreateProjectFileRequest(
    string FileName,
    string StorageKey,
    long FileSizeBytes,
    string? ContentType,
    string? Description,
    DateTime? UploadedAt);
