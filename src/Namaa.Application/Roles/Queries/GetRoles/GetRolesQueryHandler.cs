using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Roles.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Roles.Queries.GetRoles;

public sealed class GetRolesQueryHandler(IRoleRepository repository) : IRequestHandler<GetRolesQuery, Result<IReadOnlyList<RoleDto>>>
{
    public async Task<Result<IReadOnlyList<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(cancellationToken);
        return Result.Success<IReadOnlyList<RoleDto>>(entities.Select(RoleDto.FromEntity).ToList());
    }
}

