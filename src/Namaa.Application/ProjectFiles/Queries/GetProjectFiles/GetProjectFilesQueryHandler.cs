using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectFiles.Queries.GetProjectFiles;

public sealed class GetProjectFilesQueryHandler(
    IProjectFileRepository projectFileRepository,
    IProjectRepository projectRepository)
    : IRequestHandler<GetProjectFilesQuery, Result<IReadOnlyList<ProjectFileDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectFileDto>>> Handle(
        GetProjectFilesQuery request,
        CancellationToken cancellationToken)
    {
        if (await projectRepository.FindByIdAsync(
                request.ProjectId,
                cancellationToken) is null)
        {
            return Result.Failure<IReadOnlyList<ProjectFileDto>>(
                ProjectFileErrors.ProjectNotFound);
        }

        var files = await projectFileRepository.GetByProjectIdAsync(
            request.ProjectId,
            cancellationToken);

        return Result.Success<IReadOnlyList<ProjectFileDto>>(
            files
                .Select(ProjectFileDto.FromEntity)
                .ToArray());
    }
}
