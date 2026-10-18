using FluentValidation;

namespace Namaa.Application.Notifications.Commands.MarkNotificationRead;

public sealed class MarkNotificationReadCommandValidator : AbstractValidator<MarkNotificationReadCommand>
{
    public MarkNotificationReadCommandValidator() => RuleFor(command => command.Id).GreaterThan(0);
}
