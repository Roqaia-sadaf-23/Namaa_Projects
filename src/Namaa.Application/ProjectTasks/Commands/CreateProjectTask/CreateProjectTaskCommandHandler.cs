using MediatR;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Common.Errors;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectTasks.Commands.CreateProjectTask;

public sealed class CreateProjectTaskCommandHandler(
    IProjectTaskRepository projectTaskRepository,
    IProjectRepository projectRepository,
    IProjectStageRepository projectStageRepository,
    IUserRepository userRepository,
    ICurrentUser currentUser,
    IApplicationDbContext dbContext)
    : IRequestHandler<CreateProjectTaskCommand, Result<ProjectTaskDto>>
{
    public async Task<Result<ProjectTaskDto>> Handle(
        CreateProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not long actorUserId)
            return Result.Failure<ProjectTaskDto>(ApplicationErrors.Unauthenticated);

        if (await projectRepository.FindByIdAsync(
                request.ProjectId,
                cancellationToken) is null)
        {
            return Result.Failure<ProjectTaskDto>(ProjectTaskErrors.ProjectNotFound);
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

        if (!await userRepository.ExistsByIdAsync(
                actorUserId,
                cancellationToken))
        {
            return Result.Failure<ProjectTaskDto>(ProjectTaskErrors.CreatedByUserNotFound);
        }

        if (request.AssignedToUserId.HasValue &&
            !await userRepository.ExistsByIdAsync(
                request.AssignedToUserId.Value,
                cancellationToken))
        {
            return Result.Failure<ProjectTaskDto>(ProjectTaskErrors.AssignedUserNotFound);
        }

        if (request.ParentTaskId.HasValue &&
            await projectTaskRepository.FindByIdAsync(
                request.ProjectId,
                request.ParentTaskId.Value,
                cancellationToken) is null)
        {
            return Result.Failure<ProjectTaskDto>(ProjectTaskErrors.ParentTaskNotFound);
        }

        var task = ProjectTask.Create(
            request.ProjectId,
            request.Title,
            actorUserId,
            request.Priority,
            request.Description,
            request.StartDate,
            request.DueDate,
            request.AssignedToUserId,
            request.ParentTaskId,
            request.StageId,
            request.TaskType);

        await projectTaskRepository.AddAsync(task, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectTaskDto.FromEntity(task));
    }
}
