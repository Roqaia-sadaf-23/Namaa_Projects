using MediatR;
using Namaa.Application.Customers.Dtos;
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
    string? Notes) : IRequest<Result<CustomerDto>>;
