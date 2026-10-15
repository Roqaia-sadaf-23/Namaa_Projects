using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class ServiceErrors
{
    public static readonly Error NotFound = new("Services.NotFound", "The requested service was not found.", ErrorType.NotFound);
}

