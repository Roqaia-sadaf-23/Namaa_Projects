using Namaa.Domain.Common.Results;

namespace Namaa.Application.Abstractions.FileStorage;

public sealed class FileStorageValidationException(Error error) : Exception(error.Message)
{
    public Error Error { get; } = error;
}
