using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Suppliers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Suppliers.Commands.UpdateSupplier;

public sealed class UpdateSupplierCommandHandler(ISupplierRepository repository, IApplicationDbContext dbContext) : IRequestHandler<UpdateSupplierCommand, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindForUpdateAsync(request.Id, cancellationToken);
        if (entity is null) return Result.Failure<SupplierDto>(SupplierErrors.NotFound);
        entity.Update(request.Name, request.PhoneNumber, request.Email, request.Address, request.City, request.TaxNumber, request.CommercialRegister, request.Notes);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(SupplierDto.FromEntity(entity));
    }
}

