using FluentValidation;

namespace Namaa.Application.ProjectStages.Commands.CreateProjectStage;

public sealed class CreateProjectStageCommandValidator : AbstractValidator<CreateProjectStageCommand>
{
    public CreateProjectStageCommandValidator()
    {
        RuleFor(command => command.ProjectId).GreaterThan(0);

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.StageOrder).GreaterThanOrEqualTo(0);

        RuleFor(command => command.Status)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(command => command.Description).MaximumLength(1000);

        RuleFor(command => command.EndDate)
            .Must((command, endDate) =>
                !command.StartDate.HasValue ||
                !endDate.HasValue ||
                endDate.Value >= command.StartDate.Value)
            .WithMessage("End date must be on or after start date.");
    }
}
