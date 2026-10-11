using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Customers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await customerRepository.FindForUpdateAsync(
            request.Id,
            cancellationToken);

        if (customer is null)
        {
            return Result.Failure<CustomerDto>(CustomerErrors.NotFound);
        }

        customer.Update(
            request.Name,
            request.CustomerType,
            request.Email,
            request.PhoneNumber,
            request.Address,
            request.City,
            request.TaxNumber,
            request.CommercialRegister,
            request.Source,
            request.Notes);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(CustomerDto.FromEntity(customer));
    }
}
