using FluentValidation;

namespace Namaa.Application.Services.Commands.UpdateService;

public sealed class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator() { RuleFor(x => x.Id).GreaterThan((long)0); RuleFor(x => x.Name).NotEmpty().MaximumLength(200); RuleFor(x => x.DefaultPrice).GreaterThanOrEqualTo(0); RuleFor(x => x.Category).MaximumLength(100); RuleFor(x => x.Description).MaximumLength(1000); }
}

