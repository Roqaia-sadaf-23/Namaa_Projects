using MediatR;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectFiles.Commands.UpdateProjectFile;

public sealed record UpdateProjectFileCommand(
    long ProjectId,
    long Id,
    string? Description) : IRequest<Result<ProjectFileDto>>;
