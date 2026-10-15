using FluentValidation;

namespace Namaa.Application.ProjectMembers.Commands.UpdateProjectMember;

public sealed class UpdateProjectMemberCommandValidator : AbstractValidator<UpdateProjectMemberCommand>
{
    public UpdateProjectMemberCommandValidator()
    {
        RuleFor(command => command.ProjectId).GreaterThan(0);
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.ProjectRole).MaximumLength(100);
    }
}
