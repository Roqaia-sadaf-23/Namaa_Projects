using MediatR;
using Namaa.Application.Suppliers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Suppliers.Commands.UpdateSupplier;

public sealed record UpdateSupplierCommand(long Id, string Name, string? PhoneNumber, string? Email, string? Address, string? City, string? TaxNumber, string? CommercialRegister, string? Notes) : IRequest<Result<SupplierDto>>;

