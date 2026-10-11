using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Customers.Dtos;

namespace Namaa.Application.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler(
    ICustomerRepository customerRepository)
    : IRequestHandler<GetCustomersQuery, IReadOnlyList<CustomerDto>>
{
    public async Task<IReadOnlyList<CustomerDto>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = await customerRepository.GetAllAsync(cancellationToken);

        return customers.Select(CustomerDto.FromEntity).ToArray();
    }
}
