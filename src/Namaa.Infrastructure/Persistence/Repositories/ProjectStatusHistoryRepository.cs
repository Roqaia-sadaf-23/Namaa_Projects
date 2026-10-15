using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class ProjectStatusHistoryRepository(NamaaDbContext dbContext) : IProjectStatusHistoryRepository
{
    public Task<ProjectStatusHistory?> FindByIdAsync(long projectId, long id, CancellationToken cancellationToken = default) =>
        dbContext.ProjectStatusHistoryEntries.AsNoTracking().SingleOrDefaultAsync(x => x.ProjectId == projectId && x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProjectStatusHistory>> GetAllAsync(long projectId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectStatusHistoryEntries.AsNoTracking().Where(x => x.ProjectId == projectId).OrderByDescending(x => x.ChangedAt).ThenByDescending(x => x.Id).ToListAsync(cancellationToken);
}

