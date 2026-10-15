using FluentValidation;

namespace Namaa.Application.ProjectTasks.Commands.CreateProjectTask;

public sealed class CreateProjectTaskCommandValidator : AbstractValidator<CreateProjectTaskCommand>
{
    public CreateProjectTaskCommandValidator()
    {
        RuleFor(command => command.ProjectId).GreaterThan(0);

        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(command => command.Priority).IsInEnum();
        RuleFor(command => command.Description).MaximumLength(2000);

        RuleFor(command => command.AssignedToUserId)
            .GreaterThan(0)
            .When(command => command.AssignedToUserId.HasValue);

        RuleFor(command => command.ParentTaskId)
            .GreaterThan(0)
            .When(command => command.ParentTaskId.HasValue);

        RuleFor(command => command.StageId)
            .GreaterThan(0)
            .When(command => command.StageId.HasValue);

        RuleFor(command => command.TaskType)
            .MaximumLength(20)
            .Must(taskType => taskType is null or "Administrative" or "Technical")
            .WithMessage("Task type must be Administrative or Technical when provided.");

        RuleFor(command => command.DueDate)
            .Must((command, dueDate) =>
                !command.StartDate.HasValue ||
                !dueDate.HasValue ||
                dueDate.Value >= command.StartDate.Value)
            .WithMessage("Due date must be on or after start date.");
    }
}
