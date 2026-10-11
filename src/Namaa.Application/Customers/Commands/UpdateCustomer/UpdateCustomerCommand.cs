using FluentValidation;
using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Customers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    long Id,
    string Name,
    string CustomerType,
    string? Email,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? TaxNumber,
    string? CommercialRegister,
    string? Source,
    string? Notes) : IRequest<Result<CustomerDto>>;

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.CustomerType)
            .NotEmpty()
            .MaximumLength(20)
            .Must(customerType => customerType is "Company" or "Individual")
            .WithMessage("Customer type must be either 'Company' or 'Individual'.");

        RuleFor(command => command.Email)
            .MaximumLength(254)
            .EmailAddress()
            .When(command => !string.IsNullOrWhiteSpace(command.Email));
        RuleFor(command => command.PhoneNumber).MaximumLength(30);
        RuleFor(command => command.Address).MaximumLength(500);
        RuleFor(command => command.City).MaximumLength(100);
        RuleFor(command => command.TaxNumber).MaximumLength(50);
        RuleFor(command => command.CommercialRegister).MaximumLength(50);
        RuleFor(command => command.Source).MaximumLength(100);
        RuleFor(command => command.Notes).MaximumLength(1000);
    }
}

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
