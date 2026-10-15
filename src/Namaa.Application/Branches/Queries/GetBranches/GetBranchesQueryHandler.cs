using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Branches.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Branches.Queries.GetBranches;

public sealed class GetBranchesQueryHandler(IBranchRepository repository) : IRequestHandler<GetBranchesQuery, Result<IReadOnlyList<BranchDto>>>
{
    public async Task<Result<IReadOnlyList<BranchDto>>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(cancellationToken);
        return Result.Success<IReadOnlyList<BranchDto>>(entities.Select(BranchDto.FromEntity).ToList());
    }
}

