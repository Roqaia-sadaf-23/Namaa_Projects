using FluentValidation;

namespace Namaa.Application.Customers.Commands.UpdateCustomer;

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
