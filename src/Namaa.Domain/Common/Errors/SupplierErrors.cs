using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class SupplierErrors
{
    public static readonly Error NotFound = new("Suppliers.NotFound", "The requested supplier was not found.", ErrorType.NotFound);
    public static readonly Error Duplicate = new("Suppliers.DuplicateSupplierCode", "A supplier with the same supplier code already exists.", ErrorType.Conflict);
}

