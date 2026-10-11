using MediatR;
using Namaa.Application.Customers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string CustomerCode,
    string Name,
    string CustomerType = "Company") : IRequest<Result<CustomerDto>>;
