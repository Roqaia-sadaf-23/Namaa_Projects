using MediatR;
using Namaa.Application.Suppliers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Suppliers.Queries.GetSupplierById;

public sealed record GetSupplierByIdQuery(long Id) : IRequest<Result<SupplierDto>>;

