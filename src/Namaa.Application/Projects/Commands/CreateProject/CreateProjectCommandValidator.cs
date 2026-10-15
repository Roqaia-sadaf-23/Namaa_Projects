using FluentValidation;

namespace Namaa.Application.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(command => command.CustomerId).GreaterThan(0);

        RuleFor(command => command.ProjectCode)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(command => command.BudgetAmount)
            .GreaterThanOrEqualTo(0)
            .PrecisionScale(19, 4, false);

        RuleFor(command => command.SourceQuotationId)
            .GreaterThan(0)
            .When(command => command.SourceQuotationId.HasValue);

        RuleFor(command => command.ProjectManagerId)
            .GreaterThan(0)
            .When(command => command.ProjectManagerId.HasValue);

        RuleFor(command => command.Description).MaximumLength(2000);
        RuleFor(command => command.Location).MaximumLength(500);

        RuleFor(command => command.EndDate)
            .Must((command, endDate) =>
                !command.StartDate.HasValue ||
                !endDate.HasValue ||
                endDate.Value >= command.StartDate.Value)
            .WithMessage("End date must be on or after start date.");
    }
}
