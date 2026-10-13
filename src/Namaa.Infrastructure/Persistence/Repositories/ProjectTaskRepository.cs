using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class ProjectTaskRepository(NamaaDbContext dbContext) : IProjectTaskRepository
{
    public Task<ProjectTask?> FindByIdAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectTasks
            .AsNoTracking()
            .SingleOrDefaultAsync(
                task => task.ProjectId == projectId && task.Id == id,
                cancellationToken);

    public Task<ProjectTask?> FindForUpdateAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectTasks
            .SingleOrDefaultAsync(
                task => task.ProjectId == projectId && task.Id == id,
                cancellationToken);

    public async Task<IReadOnlyList<ProjectTask>> GetByProjectIdAsync(
        long projectId,
        long? stageId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectTasks
            .AsNoTracking()
            .Where(task =>
                task.ProjectId == projectId &&
                (!stageId.HasValue || task.StageId == stageId.Value))
            .OrderBy(task => task.DueDate)
            .ThenBy(task => task.Id)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(
        ProjectTask task,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectTasks.AddAsync(task, cancellationToken);
}
