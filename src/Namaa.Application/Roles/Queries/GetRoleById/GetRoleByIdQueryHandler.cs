using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Roles.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryHandler(IRoleRepository repository) : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindByIdAsync(request.Id, cancellationToken);
        return entity is null ? Result.Failure<RoleDto>(RoleErrors.NotFound) : Result.Success(RoleDto.FromEntity(entity));
    }
}

