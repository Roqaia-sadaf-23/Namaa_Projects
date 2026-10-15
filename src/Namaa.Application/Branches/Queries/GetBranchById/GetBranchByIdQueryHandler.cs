using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Branches.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Branches.Queries.GetBranchById;

public sealed class GetBranchByIdQueryHandler(IBranchRepository repository) : IRequestHandler<GetBranchByIdQuery, Result<BranchDto>>
{
    public async Task<Result<BranchDto>> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindByIdAsync(request.Id, cancellationToken);
        return entity is null ? Result.Failure<BranchDto>(BranchErrors.NotFound) : Result.Success(BranchDto.FromEntity(entity));
    }
}

