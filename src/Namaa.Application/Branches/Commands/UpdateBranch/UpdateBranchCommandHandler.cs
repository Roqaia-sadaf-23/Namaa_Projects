using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Branches.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Branches.Commands.UpdateBranch;

public sealed class UpdateBranchCommandHandler(IBranchRepository repository, IApplicationDbContext dbContext) : IRequestHandler<UpdateBranchCommand, Result<BranchDto>>
{
    public async Task<Result<BranchDto>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindForUpdateAsync(request.Id, cancellationToken);
        if (entity is null) return Result.Failure<BranchDto>(BranchErrors.NotFound);
        entity.Update(request.Name, request.Address, request.City);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(BranchDto.FromEntity(entity));
    }
}

