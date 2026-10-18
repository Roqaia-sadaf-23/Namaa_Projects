using MediatR;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectFiles.Commands.UploadProjectFile;

public sealed record UploadProjectFileCommand(
    long ProjectId,
    Stream Content,
    string FileName,
    string? ContentType,
    long FileSizeBytes,
    string? Description) : IRequest<Result<ProjectFileDto>>;
