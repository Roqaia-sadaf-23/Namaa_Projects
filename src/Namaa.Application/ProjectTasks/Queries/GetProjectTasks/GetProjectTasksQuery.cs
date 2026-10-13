using MediatR;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectTasks.Queries.GetProjectTasks;

public sealed record GetProjectTasksQuery(
    long ProjectId,
    long? StageId) : IRequest<Result<IReadOnlyList<ProjectTaskDto>>>;
