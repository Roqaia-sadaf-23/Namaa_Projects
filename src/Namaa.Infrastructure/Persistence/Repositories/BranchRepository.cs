using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class BranchRepository(NamaaDbContext dbContext) : IBranchRepository
{
    public Task<bool> ExistsByBranchCodeAsync(string value, long? excludingId = null, CancellationToken cancellationToken = default) =>
        dbContext.Branches.AsNoTracking().AnyAsync(e => e.BranchCode == value && (!excludingId.HasValue || e.Id != excludingId.Value), cancellationToken);

    public Task<Branch?> FindByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Branches.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Branch?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Branches.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Branch>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Branches.AsNoTracking().OrderBy(e => e.Id).ToListAsync(cancellationToken);

    public async Task AddAsync(Branch entity, CancellationToken cancellationToken = default) =>
        await dbContext.Branches.AddAsync(entity, cancellationToken);
}

