using Namaa.Domain.Common.Results;
namespace Namaa.Domain.Common.Errors;
public static class UserErrors{public static readonly Error NotFound=new("Users.NotFound","The user was not found.",ErrorType.NotFound);public static readonly Error DuplicateContact=new("Users.DuplicateContact","The email or phone number is already used.",ErrorType.Conflict);public static readonly Error RoleNotFound=new("Users.RoleNotFound","The role was not found.",ErrorType.NotFound);}
