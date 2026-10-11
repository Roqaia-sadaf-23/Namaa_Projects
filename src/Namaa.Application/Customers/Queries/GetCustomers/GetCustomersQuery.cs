using MediatR;
using Namaa.Application.Customers.Dtos;

namespace Namaa.Application.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery : IRequest<IReadOnlyList<CustomerDto>>;
