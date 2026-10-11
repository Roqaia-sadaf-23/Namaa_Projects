using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class ProjectStageRepository(NamaaDbContext dbContext) : IProjectStageRepository
{
    public Task<ProjectStage?> FindByIdAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectStages
            .AsNoTracking()
            .SingleOrDefaultAsync(
                stage => stage.ProjectId == projectId && stage.Id == id,
                cancellationToken);

    public Task<ProjectStage?> FindForUpdateAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectStages
            .SingleOrDefaultAsync(
                stage => stage.ProjectId == projectId && stage.Id == id,
                cancellationToken);

    public async Task<IReadOnlyList<ProjectStage>> GetByProjectIdAsync(
        long projectId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectStages
            .AsNoTracking()
            .Where(stage => stage.ProjectId == projectId)
            .OrderBy(stage => stage.StageOrder)
            .ThenBy(stage => stage.Id)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(
        ProjectStage stage,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectStages.AddAsync(stage, cancellationToken);
}
