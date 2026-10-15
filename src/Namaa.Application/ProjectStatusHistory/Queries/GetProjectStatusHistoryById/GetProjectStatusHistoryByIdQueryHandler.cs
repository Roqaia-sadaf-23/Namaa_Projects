using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectStatusHistory.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStatusHistory.Queries.GetProjectStatusHistoryById;

public sealed class GetProjectStatusHistoryByIdQueryHandler(IProjectStatusHistoryRepository repository) : IRequestHandler<GetProjectStatusHistoryByIdQuery, Result<ProjectStatusHistoryDto>>
{
    public async Task<Result<ProjectStatusHistoryDto>> Handle(GetProjectStatusHistoryByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindByIdAsync(request.ProjectId, request.Id, cancellationToken);
        return entity is null ? Result.Failure<ProjectStatusHistoryDto>(ProjectStatusHistoryErrors.NotFound) : Result.Success(ProjectStatusHistoryDto.FromEntity(entity));
    }
}

