using FluentValidation;

namespace Namaa.Application.Customers.Commands.CreateCustomer;

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
