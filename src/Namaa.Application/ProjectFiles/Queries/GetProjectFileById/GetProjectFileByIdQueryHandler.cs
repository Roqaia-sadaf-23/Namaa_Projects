using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectFiles.Queries.GetProjectFileById;

public sealed class GetProjectFileByIdQueryHandler(
    IProjectFileRepository projectFileRepository)
    : IRequestHandler<GetProjectFileByIdQuery, Result<ProjectFileDto>>
{
    public async Task<Result<ProjectFileDto>> Handle(
        GetProjectFileByIdQuery request,
        CancellationToken cancellationToken)
    {
        var file = await projectFileRepository.FindByIdAsync(
            request.ProjectId,
            request.Id,
            cancellationToken);

        return file is null
            ? Result.Failure<ProjectFileDto>(ProjectFileErrors.NotFound)
            : Result.Success(ProjectFileDto.FromEntity(file));
    }
}
