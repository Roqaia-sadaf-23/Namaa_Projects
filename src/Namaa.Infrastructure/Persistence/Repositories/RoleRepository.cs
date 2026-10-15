using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(NamaaDbContext dbContext) : IRoleRepository
{
    public Task<bool> ExistsByNameAsync(string value, byte? excludingId = null, CancellationToken cancellationToken = default) =>
        dbContext.Roles.AsNoTracking().AnyAsync(e => e.Name == value && (!excludingId.HasValue || e.Id != excludingId.Value), cancellationToken);

    public Task<Role?> FindByIdAsync(byte id, CancellationToken cancellationToken = default) =>
        dbContext.Roles.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Role?> FindForUpdateAsync(byte id, CancellationToken cancellationToken = default) =>
        dbContext.Roles.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Roles.AsNoTracking().OrderBy(e => e.Id).ToListAsync(cancellationToken);

    public async Task AddAsync(Role entity, CancellationToken cancellationToken = default) =>
        await dbContext.Roles.AddAsync(entity, cancellationToken);
}

