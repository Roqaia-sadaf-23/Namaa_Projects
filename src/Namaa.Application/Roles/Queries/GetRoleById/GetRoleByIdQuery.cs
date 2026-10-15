using MediatR;
using Namaa.Application.Roles.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdQuery(byte Id) : IRequest<Result<RoleDto>>;

