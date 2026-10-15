using MediatR;
using Namaa.Application.Roles.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Roles.Queries.GetRoles;

public sealed record GetRolesQuery : IRequest<Result<IReadOnlyList<RoleDto>>>;

