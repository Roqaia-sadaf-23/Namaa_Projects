using FluentValidation;

namespace Namaa.Application.ProjectStages.Commands.UpdateProjectStage;

public sealed class UpdateProjectStageCommandValidator : AbstractValidator<UpdateProjectStageCommand>
{
    public UpdateProjectStageCommandValidator()
    {
        RuleFor(command => command.ProjectId).GreaterThan(0);
        RuleFor(command => command.Id).GreaterThan(0);

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.StageOrder).GreaterThanOrEqualTo(0);

        RuleFor(command => command.Status)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(command => command.ProgressPercentage)
            .InclusiveBetween(0, 100)
            .PrecisionScale(5, 2, false);

        RuleFor(command => command.Description).MaximumLength(1000);

        RuleFor(command => command.EndDate)
            .Must((command, endDate) =>
                !command.StartDate.HasValue ||
                !endDate.HasValue ||
                endDate.Value >= command.StartDate.Value)
            .WithMessage("End date must be on or after start date.");
    }
}
