using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class BranchErrors
{
    public static readonly Error NotFound = new("Branches.NotFound", "The requested branch was not found.", ErrorType.NotFound);
    public static readonly Error Duplicate = new("Branches.DuplicateBranchCode", "A branch with the same branch code already exists.", ErrorType.Conflict);
}

