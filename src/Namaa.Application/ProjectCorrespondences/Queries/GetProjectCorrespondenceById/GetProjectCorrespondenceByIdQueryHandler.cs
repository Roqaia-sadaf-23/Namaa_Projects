using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectCorrespondences.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectCorrespondences.Queries.GetProjectCorrespondenceById;

public sealed class GetProjectCorrespondenceByIdQueryHandler(IProjectCorrespondenceRepository repository) : IRequestHandler<GetProjectCorrespondenceByIdQuery, Result<ProjectCorrespondenceDto>>
{
    public async Task<Result<ProjectCorrespondenceDto>> Handle(GetProjectCorrespondenceByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindByIdAsync(request.ProjectId, request.Id, cancellationToken);
        return entity is null ? Result.Failure<ProjectCorrespondenceDto>(ProjectCorrespondenceErrors.NotFound) : Result.Success(ProjectCorrespondenceDto.FromEntity(entity));
    }
}

