using MediatR;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(long Id) : IRequest<Result<ProjectDto>>;
