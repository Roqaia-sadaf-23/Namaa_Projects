using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class RoleErrors
{
    public static readonly Error NotFound = new("Roles.NotFound", "The requested role was not found.", ErrorType.NotFound);
    public static readonly Error Duplicate = new("Roles.DuplicateName", "A role with the same role name already exists.", ErrorType.Conflict);
}

