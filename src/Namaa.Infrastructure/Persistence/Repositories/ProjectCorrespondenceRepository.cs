using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class ProjectCorrespondenceRepository(NamaaDbContext dbContext) : IProjectCorrespondenceRepository
{
    public Task<ProjectCorrespondence?> FindByIdAsync(long projectId, long id, CancellationToken cancellationToken = default) =>
        dbContext.ProjectCorrespondences.AsNoTracking().SingleOrDefaultAsync(x => x.ProjectId == projectId && x.Id == id, cancellationToken);

    public Task<ProjectCorrespondence?> FindForUpdateAsync(long projectId, long id, CancellationToken cancellationToken = default) =>
        dbContext.ProjectCorrespondences.SingleOrDefaultAsync(x => x.ProjectId == projectId && x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProjectCorrespondence>> GetAllAsync(long projectId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectCorrespondences.AsNoTracking().Where(x => x.ProjectId == projectId).OrderByDescending(x => x.CorrespondenceDate).ThenBy(x => x.Id).ToListAsync(cancellationToken);

    public async Task AddAsync(ProjectCorrespondence correspondence, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectCorrespondences.AddAsync(correspondence, cancellationToken);
}

