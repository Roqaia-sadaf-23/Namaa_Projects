using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository(NamaaDbContext dbContext) : IProjectRepository
{
    public Task<bool> ExistsByCodeAsync(
        string projectCode,
        CancellationToken cancellationToken = default) =>
        dbContext.Projects
            .AsNoTracking()
            .AnyAsync(
                project => project.ProjectCode == projectCode,
                cancellationToken);

    public Task<bool> ExistsBySourceQuotationIdAsync(
        long sourceQuotationId,
        CancellationToken cancellationToken = default) =>
        dbContext.Projects
            .AsNoTracking()
            .AnyAsync(
                project => project.SourceQuotationId == sourceQuotationId,
                cancellationToken);

    public Task<Project?> FindByIdAsync(
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.Projects
            .AsNoTracking()
            .SingleOrDefaultAsync(project => project.Id == id, cancellationToken);

    public Task<Project?> FindForUpdateAsync(
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.Projects
            .SingleOrDefaultAsync(project => project.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Project>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .AsNoTracking()
            .OrderBy(project => project.Id)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(
        Project project,
        CancellationToken cancellationToken = default) =>
        await dbContext.Projects.AddAsync(project, cancellationToken);
}
