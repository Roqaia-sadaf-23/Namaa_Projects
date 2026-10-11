using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectStages.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectStages.Commands.CreateProjectStage;

public sealed class CreateProjectStageCommandHandler(
    IProjectStageRepository projectStageRepository,
    IProjectRepository projectRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<CreateProjectStageCommand, Result<ProjectStageDto>>
{
    public async Task<Result<ProjectStageDto>> Handle(
        CreateProjectStageCommand request,
        CancellationToken cancellationToken)
    {
        if (await projectRepository.FindByIdAsync(
                request.ProjectId,
                cancellationToken) is null)
        {
            return Result.Failure<ProjectStageDto>(ProjectStageErrors.ProjectNotFound);
        }

        var stage = ProjectStage.Create(
            request.ProjectId,
            request.Name,
            request.StageOrder,
            request.Status,
            request.Description,
            request.StartDate,
            request.EndDate);

        await projectStageRepository.AddAsync(stage, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectStageDto.FromEntity(stage));
    }
}
