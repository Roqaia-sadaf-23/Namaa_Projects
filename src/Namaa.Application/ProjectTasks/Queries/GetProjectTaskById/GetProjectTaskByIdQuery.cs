using MediatR;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectTasks.Queries.GetProjectTaskById;

public sealed record GetProjectTaskByIdQuery(
    long ProjectId,
    long Id) : IRequest<Result<ProjectTaskDto>>;
