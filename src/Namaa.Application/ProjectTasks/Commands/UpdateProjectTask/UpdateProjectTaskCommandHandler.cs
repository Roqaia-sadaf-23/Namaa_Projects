using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectTasks.Commands.UpdateProjectTask;

public sealed class UpdateProjectTaskCommandHandler(
    IProjectTaskRepository projectTaskRepository,
    IProjectStageRepository projectStageRepository,
    IUserRepository userRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<UpdateProjectTaskCommand, Result<ProjectTaskDto>>
{
    public async Task<Result<ProjectTaskDto>> Handle(
        UpdateProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await projectTaskRepository.FindForUpdateAsync(
            request.ProjectId,
            request.Id,
            cancellationToken);

        if (task is null)
        {
            return Result.Failure<ProjectTaskDto>(ProjectTaskErrors.NotFound);
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

                return Result.Failure<ProjectTaskDto>(
                    stageExists
                        ? ProjectTaskErrors.InvalidStageForProject
                        : ProjectTaskErrors.StageNotFound);
            }
        }

        if (request.AssignedToUserId.HasValue &&
            !await userRepository.ExistsByIdAsync(
                request.AssignedToUserId.Value,
                cancellationToken))
        {
            return Result.Failure<ProjectTaskDto>(ProjectTaskErrors.AssignedUserNotFound);
        }

        task.Update(
            request.Title,
            request.Priority,
            request.Description,
            request.StartDate,
            request.DueDate,
            request.AssignedToUserId,
            request.StageId,
            request.TaskType);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectTaskDto.FromEntity(task));
    }
}
