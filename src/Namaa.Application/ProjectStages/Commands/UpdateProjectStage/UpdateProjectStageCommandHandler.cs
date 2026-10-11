using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectStages.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStages.Commands.UpdateProjectStage;

public sealed class UpdateProjectStageCommandHandler(
    IProjectStageRepository projectStageRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<UpdateProjectStageCommand, Result<ProjectStageDto>>
{
    public async Task<Result<ProjectStageDto>> Handle(
        UpdateProjectStageCommand request,
        CancellationToken cancellationToken)
    {
        var stage = await projectStageRepository.FindForUpdateAsync(
            request.ProjectId,
            request.Id,
            cancellationToken);

        if (stage is null)
        {
            return Result.Failure<ProjectStageDto>(ProjectStageErrors.NotFound);
        }

        stage.Update(
            request.Name,
            request.StageOrder,
            request.Status,
            request.ProgressPercentage,
            request.Description,
            request.StartDate,
            request.EndDate);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectStageDto.FromEntity(stage));
    }
}
