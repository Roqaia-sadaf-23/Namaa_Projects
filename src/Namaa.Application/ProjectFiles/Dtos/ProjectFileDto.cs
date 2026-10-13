using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectFiles.Dtos;

public sealed record ProjectFileDto(
    long Id,
    long ProjectId,
    long UploadedByUserId,
    string FileName,
    string? ContentType,
    long FileSizeBytes,
    string? Description,
    DateTime UploadedAt)
{
    internal static ProjectFileDto FromEntity(ProjectFile file) =>
        new(
            file.Id,
            file.ProjectId,
            file.UploadedByUserId,
            file.FileName,
            file.ContentType,
            file.FileSizeBytes,
            file.Description,
            file.UploadedAt);
}
