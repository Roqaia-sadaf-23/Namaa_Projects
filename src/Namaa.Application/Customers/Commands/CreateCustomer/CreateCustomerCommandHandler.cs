using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Customers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        if (await customerRepository.ExistsByCodeAsync(
                request.CustomerCode,
                cancellationToken))
        {
            return Result.Failure<CustomerDto>(CustomerErrors.DuplicateCustomerCode);
        }

        var customer = Customer.Create(
            request.CustomerCode,
            request.Name,
            request.CustomerType);

        await customerRepository.AddAsync(customer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(CustomerDto.FromEntity(customer));
    }
}
