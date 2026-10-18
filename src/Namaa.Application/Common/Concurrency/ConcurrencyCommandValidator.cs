using FluentValidation;

namespace Namaa.Application.Common.Concurrency;

public sealed class ConcurrencyCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : IConcurrencyCommand
{
    public ConcurrencyCommandValidator()
    {
        RuleFor(command => command.RowVersion)
            .NotEmpty()
            .Must(value => ConcurrencyToken.TryDecode(value, out _))
            .WithMessage("RowVersion must be a valid Base64 value that decodes to exactly 8 bytes.");
    }
}
