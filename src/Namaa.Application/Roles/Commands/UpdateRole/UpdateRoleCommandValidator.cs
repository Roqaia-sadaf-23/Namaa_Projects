using FluentValidation;

namespace Namaa.Application.Roles.Commands.UpdateRole;

public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator() { RuleFor(x => x.Id).GreaterThan((byte)0); RuleFor(x => x.Name).NotEmpty().MaximumLength(30); RuleFor(x => x.Description).MaximumLength(250); }
}

