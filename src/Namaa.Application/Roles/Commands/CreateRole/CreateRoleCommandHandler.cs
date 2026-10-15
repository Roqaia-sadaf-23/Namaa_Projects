using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Roles.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler(IRoleRepository repository, IApplicationDbContext dbContext) : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await repository.ExistsByNameAsync(request.Name, null, cancellationToken)) return Result.Failure<RoleDto>(RoleErrors.Duplicate);
        var entity = Role.Create(request.Name, request.Description);
        await repository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(RoleDto.FromEntity(entity));
    }
}

