namespace Namaa.Api.Contracts.Users;public sealed record UpdateUserRequest(byte RoleId,string FullName,string? Email,string? PhoneNumber,bool IsActive);
