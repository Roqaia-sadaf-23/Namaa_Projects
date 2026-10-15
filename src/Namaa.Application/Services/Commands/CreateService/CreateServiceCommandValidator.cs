using FluentValidation;

namespace Namaa.Application.Services.Commands.CreateService;

public sealed class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator() { RuleFor(x => x.Name).NotEmpty().MaximumLength(200); RuleFor(x => x.DefaultPrice).GreaterThanOrEqualTo(0); RuleFor(x => x.Category).MaximumLength(100); RuleFor(x => x.Description).MaximumLength(1000); }
}

