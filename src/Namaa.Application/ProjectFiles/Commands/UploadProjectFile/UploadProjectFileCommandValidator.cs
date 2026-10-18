using FluentValidation;

namespace Namaa.Application.ProjectFiles.Commands.UploadProjectFile;

public sealed class UploadProjectFileCommandValidator : AbstractValidator<UploadProjectFileCommand>
{
    public UploadProjectFileCommandValidator()
    {
        RuleFor(command => command.ProjectId).GreaterThan(0);
        RuleFor(command => command.Content).NotNull();
        RuleFor(command => command.FileName).NotEmpty().MaximumLength(1024);
        RuleFor(command => command.FileSizeBytes).GreaterThan(0);
        RuleFor(command => command.ContentType).MaximumLength(256);
        RuleFor(command => command.Description).MaximumLength(500);
    }
}
