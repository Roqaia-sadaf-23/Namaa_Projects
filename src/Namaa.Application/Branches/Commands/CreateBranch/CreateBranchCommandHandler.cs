using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Branches.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.Branches.Commands.CreateBranch;

public sealed class CreateBranchCommandHandler(IBranchRepository repository, IApplicationDbContext dbContext) : IRequestHandler<CreateBranchCommand, Result<BranchDto>>
{
    public async Task<Result<BranchDto>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        if (await repository.ExistsByBranchCodeAsync(request.BranchCode, null, cancellationToken)) return Result.Failure<BranchDto>(BranchErrors.Duplicate);
        var entity = Branch.Create(request.BranchCode, request.Name, request.Address, request.City);
        await repository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(BranchDto.FromEntity(entity));
    }
}

