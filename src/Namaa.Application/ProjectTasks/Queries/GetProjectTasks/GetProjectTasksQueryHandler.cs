using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectTasks.Queries.GetProjectTasks;

public sealed class GetProjectTasksQueryHandler(
    IProjectTaskRepository projectTaskRepository,
    IProjectRepository projectRepository,
    IProjectStageRepository projectStageRepository)
    : IRequestHandler<GetProjectTasksQuery, Result<IReadOnlyList<ProjectTaskDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectTaskDto>>> Handle(
        GetProjectTasksQuery request,
        CancellationToken cancellationToken)
    {
        if (await projectRepository.FindByIdAsync(
                request.ProjectId,
                cancellationToken) is null)
        {
            return Result.Failure<IReadOnlyList<ProjectTaskDto>>(
                ProjectTaskErrors.ProjectNotFound);
        }

        if (request.StageId.HasValue)
        {
            var stage = await projectStageRepository.FindByIdAsync(
                request.ProjectId,
                request.StageId.Value,
                cancellationToken);

            if (stage is null)
            {
                var stageExists = await projectStageRepository.ExistsByIdAsync(
                    request.StageId.Value,
                    cancellationToken);

                return Result.Failure<IReadOnlyList<ProjectTaskDto>>(
                    stageExists
                        ? ProjectTaskErrors.InvalidStageForProject
                        : ProjectTaskErrors.StageNotFound);
            }
        }

        var tasks = await projectTaskRepository.GetByProjectIdAsync(
            request.ProjectId,
            request.StageId,
            cancellationToken);

        return Result.Success<IReadOnlyList<ProjectTaskDto>>(
            tasks
                .Select(ProjectTaskDto.FromEntity)
                .ToArray());
    }
}
