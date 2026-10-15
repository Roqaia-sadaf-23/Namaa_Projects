using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectStatusHistory.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStatusHistory.Queries.GetProjectStatusHistory;

public sealed class GetProjectStatusHistoryQueryHandler(IProjectStatusHistoryRepository repository) : IRequestHandler<GetProjectStatusHistoryQuery, Result<IReadOnlyList<ProjectStatusHistoryDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectStatusHistoryDto>>> Handle(GetProjectStatusHistoryQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(request.ProjectId, cancellationToken);
        return Result.Success<IReadOnlyList<ProjectStatusHistoryDto>>(entities.Select(ProjectStatusHistoryDto.FromEntity).ToList());
    }
}

