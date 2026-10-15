using FluentValidation;

namespace Namaa.Application.ProjectFiles.Commands.CreateProjectFile;

public sealed class CreateProjectFileCommandValidator : AbstractValidator<CreateProjectFileCommand>
{
    public CreateProjectFileCommandValidator()
    {
        RuleFor(command => command.ProjectId).GreaterThan(0);

        RuleFor(command => command.FileName)
            .NotEmpty()
            .MaximumLength(260);

        RuleFor(command => command.StorageKey)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(command => command.FileSizeBytes).GreaterThan(0);
        RuleFor(command => command.ContentType).MaximumLength(100);
        RuleFor(command => command.Description).MaximumLength(500);
    }
}
