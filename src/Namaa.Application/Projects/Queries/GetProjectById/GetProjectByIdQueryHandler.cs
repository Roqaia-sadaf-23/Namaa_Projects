using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler(
    IProjectRepository projectRepository)
    : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindByIdAsync(
            request.Id,
            cancellationToken);

        return project is null
            ? Result.Failure<ProjectDto>(ProjectErrors.NotFound)
            : Result.Success(ProjectDto.FromEntity(project));
    }
}
