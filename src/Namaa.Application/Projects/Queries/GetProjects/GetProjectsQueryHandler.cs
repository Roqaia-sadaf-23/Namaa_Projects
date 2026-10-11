using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Projects.Queries.GetProjects;

public sealed class GetProjectsQueryHandler(
    IProjectRepository projectRepository)
    : IRequestHandler<GetProjectsQuery, Result<IReadOnlyList<ProjectDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectDto>>> Handle(
        GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await projectRepository.GetAllAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ProjectDto>>(
            projects
                .Select(ProjectDto.FromEntity)
                .ToArray());
    }
}
