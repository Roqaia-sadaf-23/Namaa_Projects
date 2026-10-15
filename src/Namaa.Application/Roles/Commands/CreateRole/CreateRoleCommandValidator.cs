using FluentValidation;

namespace Namaa.Application.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator() { RuleFor(x => x.Name).NotEmpty().MaximumLength(30); RuleFor(x => x.Description).MaximumLength(250); }
}

