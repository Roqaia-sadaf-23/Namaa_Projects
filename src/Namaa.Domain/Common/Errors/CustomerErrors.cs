using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class CustomerErrors
{
    public static readonly Error NotFound = new(
        "Customers.NotFound",
        "The requested customer was not found.",
        ErrorType.NotFound);

    public static readonly Error DuplicateCustomerCode = new(
        "Customers.DuplicateCustomerCode",
        "A customer with the same customer code already exists.",
        ErrorType.Conflict);
}
