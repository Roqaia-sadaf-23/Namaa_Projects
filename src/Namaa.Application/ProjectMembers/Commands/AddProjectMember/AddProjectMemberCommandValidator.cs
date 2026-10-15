using FluentValidation;

namespace Namaa.Application.ProjectMembers.Commands.AddProjectMember;

public sealed class AddProjectMemberCommandValidator : AbstractValidator<AddProjectMemberCommand>
{
    public AddProjectMemberCommandValidator()
    {
        RuleFor(command => command.ProjectId).GreaterThan(0);
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.ProjectRole).MaximumLength(100);
    }
}
