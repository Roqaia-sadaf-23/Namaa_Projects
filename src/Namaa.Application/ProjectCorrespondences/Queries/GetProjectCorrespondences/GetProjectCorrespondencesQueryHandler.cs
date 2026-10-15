using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectCorrespondences.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectCorrespondences.Queries.GetProjectCorrespondences;

public sealed class GetProjectCorrespondencesQueryHandler(IProjectCorrespondenceRepository repository) : IRequestHandler<GetProjectCorrespondencesQuery, Result<IReadOnlyList<ProjectCorrespondenceDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectCorrespondenceDto>>> Handle(GetProjectCorrespondencesQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(request.ProjectId, cancellationToken);
        return Result.Success<IReadOnlyList<ProjectCorrespondenceDto>>(entities.Select(ProjectCorrespondenceDto.FromEntity).ToList());
    }
}

