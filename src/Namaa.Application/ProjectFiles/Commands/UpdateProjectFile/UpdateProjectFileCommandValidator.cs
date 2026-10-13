using FluentValidation;

namespace Namaa.Application.ProjectFiles.Commands.UpdateProjectFile;

public sealed class UpdateProjectFileCommandValidator : AbstractValidator<UpdateProjectFileCommand>
{
    public UpdateProjectFileCommandValidator()
    {
        RuleFor(command => command.ProjectId).GreaterThan(0);
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Description).MaximumLength(500);
    }
}
