using FluentValidation;
using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Customers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string CustomerCode,
    string Name,
    string CustomerType = "Company") : IRequest<Result<CustomerDto>>;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(command => command.CustomerCode)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.CustomerType)
            .NotEmpty()
            .MaximumLength(20)
            .Must(customerType => customerType is "Company" or "Individual")
            .WithMessage("Customer type must be either 'Company' or 'Individual'.");
    }
}

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
