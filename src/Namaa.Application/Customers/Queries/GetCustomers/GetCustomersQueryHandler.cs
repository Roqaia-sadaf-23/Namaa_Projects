using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Customers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler(
    ICustomerRepository customerRepository)
    : IRequestHandler<GetCustomersQuery, Result<IReadOnlyList<CustomerDto>>>
{
    public async Task<Result<IReadOnlyList<CustomerDto>>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = await customerRepository.GetAllAsync(cancellationToken);

        return Result.Success<IReadOnlyList<CustomerDto>>(
            customers
                .Select(CustomerDto.FromEntity)
                .ToArray());
    }
}
