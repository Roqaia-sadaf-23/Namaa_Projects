#pragma warning disable CA1716 // "Error" is the conventional public name for the requested Result Pattern.
namespace Namaa.Application.Common.Results;

public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
}
#pragma warning restore CA1716
