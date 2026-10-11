using FluentValidation;
using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Customers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(long Id) : IRequest<Result<CustomerDto>>;

public sealed class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(query => query.Id).GreaterThan(0);
    }
}

public sealed class GetCustomerByIdQueryHandler(
    ICustomerRepository customerRepository)
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await customerRepository.FindByIdAsync(
            request.Id,
            cancellationToken);

        return customer is null
            ? Result.Failure<CustomerDto>(CustomerErrors.NotFound)
            : Result.Success(CustomerDto.FromEntity(customer));
    }
}
