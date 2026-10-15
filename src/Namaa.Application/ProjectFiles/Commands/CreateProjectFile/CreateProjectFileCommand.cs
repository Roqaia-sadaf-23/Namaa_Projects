using MediatR;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectFiles.Commands.CreateProjectFile;

public sealed record CreateProjectFileCommand(
    long ProjectId,
    string FileName,
    string StorageKey,
    long FileSizeBytes,
    string? ContentType,
    string? Description,
    DateTime? UploadedAt) : IRequest<Result<ProjectFileDto>>;
