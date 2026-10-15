using MediatR;
using Namaa.Application.Customers.Dtos;
using Namaa.Application.Common.Concurrency;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    long Id,
    string Name,
    string CustomerType,
    string? Email,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? TaxNumber,
    string? CommercialRegister,
    string? Source,
    string? Notes,
    string RowVersion) : IRequest<Result<CustomerDto>>, IConcurrencyCommand;
