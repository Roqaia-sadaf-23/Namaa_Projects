using MediatR;
using Namaa.Application.Suppliers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Suppliers.Queries.GetSuppliers;

public sealed record GetSuppliersQuery : IRequest<Result<IReadOnlyList<SupplierDto>>>;

