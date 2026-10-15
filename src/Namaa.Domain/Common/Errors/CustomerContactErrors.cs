using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class CustomerContactErrors
{
    public static readonly Error CustomerNotFound = new("CustomerContacts.CustomerNotFound", "The customer was not found.", ErrorType.NotFound);
    public static readonly Error NotFound = new("CustomerContacts.NotFound", "The requested customer contact was not found.", ErrorType.NotFound);
    public static readonly Error PrimaryAlreadyExists = new("CustomerContacts.PrimaryAlreadyExists", "The customer already has a primary contact.", ErrorType.Conflict);
}

