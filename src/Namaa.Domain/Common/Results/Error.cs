#pragma warning disable CA1716 // Error is the conventional name for this Result Pattern type.
namespace Namaa.Domain.Common.Results;

public sealed record Error
{
    private Error(string code, string message, ErrorType type, bool isNone)
    {
        if (!isNone)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);
        }

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "The error type is not defined.");
        }

        Code = code;
        Message = message;
        Type = type;
    }

    public Error(string code, string message, ErrorType type)
        : this(code, message, type, false)
    {
    }

    public static Error None { get; } = new(string.Empty, string.Empty, ErrorType.Failure, true);

    public string Code { get; }

    public string Message { get; }

    public ErrorType Type { get; }
}
#pragma warning restore CA1716
