using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Suppliers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.Suppliers.Commands.CreateSupplier;

public sealed class CreateSupplierCommandHandler(ISupplierRepository repository, IApplicationDbContext dbContext) : IRequestHandler<CreateSupplierCommand, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        if (await repository.ExistsBySupplierCodeAsync(request.SupplierCode, null, cancellationToken)) return Result.Failure<SupplierDto>(SupplierErrors.Duplicate);
        var entity = Supplier.Create(request.SupplierCode, request.Name);
        await repository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(SupplierDto.FromEntity(entity));
    }
}

