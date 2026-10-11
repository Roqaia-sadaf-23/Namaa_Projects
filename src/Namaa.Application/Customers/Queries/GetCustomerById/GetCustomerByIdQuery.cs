using MediatR;
using Namaa.Application.Customers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(long Id) : IRequest<Result<CustomerDto>>;
