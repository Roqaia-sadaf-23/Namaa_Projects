using FluentValidation;

namespace Namaa.Application.Projects.Commands.ChangeProjectStatus;

public sealed class ChangeProjectStatusCommandValidator : AbstractValidator<ChangeProjectStatusCommand>
{
    public ChangeProjectStatusCommandValidator() { RuleFor(x => x.ProjectId).GreaterThan(0); RuleFor(x => x.Status).IsInEnum(); RuleFor(x => x.ChangedByUserId).GreaterThan(0); RuleFor(x => x.Reason).MaximumLength(1000); }
}

