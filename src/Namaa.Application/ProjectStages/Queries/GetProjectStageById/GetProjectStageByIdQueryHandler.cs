using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectStages.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStages.Queries.GetProjectStageById;

public sealed class GetProjectStageByIdQueryHandler(
    IProjectStageRepository projectStageRepository)
    : IRequestHandler<GetProjectStageByIdQuery, Result<ProjectStageDto>>
{
    public async Task<Result<ProjectStageDto>> Handle(
        GetProjectStageByIdQuery request,
        CancellationToken cancellationToken)
    {
        var stage = await projectStageRepository.FindByIdAsync(
            request.ProjectId,
            request.Id,
            cancellationToken);

        return stage is null
            ? Result.Failure<ProjectStageDto>(ProjectStageErrors.NotFound)
            : Result.Success(ProjectStageDto.FromEntity(stage));
    }
}
