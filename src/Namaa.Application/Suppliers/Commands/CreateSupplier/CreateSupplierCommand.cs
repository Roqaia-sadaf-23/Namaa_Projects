using MediatR;
using Namaa.Application.Suppliers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Suppliers.Commands.CreateSupplier;

public sealed record CreateSupplierCommand(string SupplierCode, string Name) : IRequest<Result<SupplierDto>>;

