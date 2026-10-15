using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Suppliers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Suppliers.Queries.GetSuppliers;

public sealed class GetSuppliersQueryHandler(ISupplierRepository repository) : IRequestHandler<GetSuppliersQuery, Result<IReadOnlyList<SupplierDto>>>
{
    public async Task<Result<IReadOnlyList<SupplierDto>>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(cancellationToken);
        return Result.Success<IReadOnlyList<SupplierDto>>(entities.Select(SupplierDto.FromEntity).ToList());
    }
}

