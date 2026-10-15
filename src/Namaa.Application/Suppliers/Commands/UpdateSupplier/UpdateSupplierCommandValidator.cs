using FluentValidation;

namespace Namaa.Application.Suppliers.Commands.UpdateSupplier;

public sealed class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator() { RuleFor(x => x.Id).GreaterThan((long)0); RuleFor(x => x.Name).NotEmpty().MaximumLength(200); RuleFor(x => x.PhoneNumber).MaximumLength(30); RuleFor(x => x.Email).MaximumLength(254).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)); RuleFor(x => x.Address).MaximumLength(500); RuleFor(x => x.City).MaximumLength(100); RuleFor(x => x.TaxNumber).MaximumLength(50); RuleFor(x => x.CommercialRegister).MaximumLength(50); RuleFor(x => x.Notes).MaximumLength(1000); }
}

