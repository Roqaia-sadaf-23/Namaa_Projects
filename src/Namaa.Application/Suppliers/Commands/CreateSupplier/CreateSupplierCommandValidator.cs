using FluentValidation;

namespace Namaa.Application.Suppliers.Commands.CreateSupplier;

public sealed class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator() { RuleFor(x => x.SupplierCode).NotEmpty().MaximumLength(30); RuleFor(x => x.Name).NotEmpty().MaximumLength(200); }
}

