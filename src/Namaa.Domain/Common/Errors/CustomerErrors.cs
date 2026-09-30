using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class CustomerErrors
{
    public static readonly Error NotFound = new(
        "Customers.NotFound",
        "The requested customer was not found.",
        ErrorType.NotFound);

    public static readonly Error DuplicateNationalId = new(
        "Customers.DuplicateNationalId",
        "A customer with the same national ID already exists.",
        ErrorType.Conflict);
}
