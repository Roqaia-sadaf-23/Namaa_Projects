using MediatR;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Projects.Queries.GetProjects;

public sealed record GetProjectsQuery : IRequest<Result<IReadOnlyList<ProjectDto>>>;
