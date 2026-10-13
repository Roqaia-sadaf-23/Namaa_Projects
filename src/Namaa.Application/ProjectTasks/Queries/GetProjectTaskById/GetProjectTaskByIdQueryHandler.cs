using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectTasks.Queries.GetProjectTaskById;

public sealed class GetProjectTaskByIdQueryHandler(
    IProjectTaskRepository projectTaskRepository)
    : IRequestHandler<GetProjectTaskByIdQuery, Result<ProjectTaskDto>>
{
    public async Task<Result<ProjectTaskDto>> Handle(
        GetProjectTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        var task = await projectTaskRepository.FindByIdAsync(
            request.ProjectId,
            request.Id,
            cancellationToken);

        return task is null
            ? Result.Failure<ProjectTaskDto>(ProjectTaskErrors.NotFound)
            : Result.Success(ProjectTaskDto.FromEntity(task));
    }
}
