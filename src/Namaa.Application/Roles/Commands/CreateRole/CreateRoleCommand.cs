using MediatR;
using Namaa.Application.Roles.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(string Name, string? Description) : IRequest<Result<RoleDto>>;

