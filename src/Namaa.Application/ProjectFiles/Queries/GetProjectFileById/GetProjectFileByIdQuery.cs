using MediatR;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectFiles.Queries.GetProjectFileById;

public sealed record GetProjectFileByIdQuery(
    long ProjectId,
    long Id) : IRequest<Result<ProjectFileDto>>;
