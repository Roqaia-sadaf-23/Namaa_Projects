using MediatR;
using Namaa.Application.Customers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery : IRequest<Result<IReadOnlyList<CustomerDto>>>;
