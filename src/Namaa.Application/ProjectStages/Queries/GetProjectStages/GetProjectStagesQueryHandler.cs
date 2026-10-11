using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectStages.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStages.Queries.GetProjectStages;

public sealed class GetProjectStagesQueryHandler(
    IProjectStageRepository projectStageRepository,
    IProjectRepository projectRepository)
    : IRequestHandler<GetProjectStagesQuery, Result<IReadOnlyList<ProjectStageDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectStageDto>>> Handle(
        GetProjectStagesQuery request,
        CancellationToken cancellationToken)
    {
        if (await projectRepository.FindByIdAsync(
                request.ProjectId,
                cancellationToken) is null)
        {
            return Result.Failure<IReadOnlyList<ProjectStageDto>>(
                ProjectStageErrors.ProjectNotFound);
        }

        var stages = await projectStageRepository.GetByProjectIdAsync(
            request.ProjectId,
            cancellationToken);

        return Result.Success<IReadOnlyList<ProjectStageDto>>(
            stages
                .Select(ProjectStageDto.FromEntity)
                .ToArray());
    }
}
