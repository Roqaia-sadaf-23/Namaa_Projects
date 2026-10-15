using FluentValidation;

namespace Namaa.Application.Authentication.Commands.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator() =>
        RuleFor(command => command.RefreshToken)
            .NotEmpty()
            .MaximumLength(512);
}
