using Namaa.Domain.Entities;

namespace Namaa.Application.Roles.Dtos;

public sealed record RoleDto(byte Id, string Name, string? Description)
{
    internal static RoleDto FromEntity(Role e) => new(e.Id, e.Name, e.Description);
}

