using MediatR;
using Namaa.Application.Roles.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Roles.Commands.UpdateRole;

public sealed record UpdateRoleCommand(byte Id, string Name, string? Description) : IRequest<Result<RoleDto>>;

