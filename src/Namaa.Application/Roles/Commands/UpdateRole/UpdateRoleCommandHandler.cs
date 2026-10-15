using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Roles.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Roles.Commands.UpdateRole;

public sealed class UpdateRoleCommandHandler(IRoleRepository repository, IApplicationDbContext dbContext) : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindForUpdateAsync(request.Id, cancellationToken);
        if (entity is null) return Result.Failure<RoleDto>(RoleErrors.NotFound);
        if (await repository.ExistsByNameAsync(request.Name, request.Id, cancellationToken)) return Result.Failure<RoleDto>(RoleErrors.Duplicate);
        entity.Update(request.Name, request.Description);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(RoleDto.FromEntity(entity));
    }
}

