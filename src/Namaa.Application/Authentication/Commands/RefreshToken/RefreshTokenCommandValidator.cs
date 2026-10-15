using FluentValidation;

namespace Namaa.Application.Authentication.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator() =>
        RuleFor(command => command.RefreshToken)
            .NotEmpty()
            .MaximumLength(512);
}
