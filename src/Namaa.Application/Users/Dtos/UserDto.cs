using Namaa.Application.Common.Concurrency;using Namaa.Domain.Entities;
namespace Namaa.Application.Users.Dtos;
public sealed record UserDto(long Id,byte RoleId,string FullName,string? Email,string? PhoneNumber,bool IsActive,DateTime CreatedAt,DateTime? UpdatedAt,string RowVersion){internal static UserDto FromEntity(User e)=>new(e.Id,e.RoleId,e.FullName,e.Email,e.PhoneNumber,e.IsActive,e.CreatedAt,e.UpdatedAt,ConcurrencyToken.Encode(e.RowVersion));}
