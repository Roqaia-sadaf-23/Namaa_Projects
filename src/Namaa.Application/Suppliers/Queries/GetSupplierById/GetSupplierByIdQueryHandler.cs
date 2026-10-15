using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Suppliers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Suppliers.Queries.GetSupplierById;

public sealed class GetSupplierByIdQueryHandler(ISupplierRepository repository) : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindByIdAsync(request.Id, cancellationToken);
        return entity is null ? Result.Failure<SupplierDto>(SupplierErrors.NotFound) : Result.Success(SupplierDto.FromEntity(entity));
    }
}

