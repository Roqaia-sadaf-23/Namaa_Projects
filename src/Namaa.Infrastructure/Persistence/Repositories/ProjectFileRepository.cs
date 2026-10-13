using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class ProjectFileRepository(NamaaDbContext dbContext) : IProjectFileRepository
{
    public Task<bool> ExistsByStorageKeyAsync(
        string storageKey,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectFiles
            .AsNoTracking()
            .AnyAsync(file => file.StorageKey == storageKey, cancellationToken);

    public Task<ProjectFile?> FindByIdAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectFiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                file => file.ProjectId == projectId && file.Id == id,
                cancellationToken);

    public Task<ProjectFile?> FindForUpdateAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectFiles
            .SingleOrDefaultAsync(
                file => file.ProjectId == projectId && file.Id == id,
                cancellationToken);

    public async Task<IReadOnlyList<ProjectFile>> GetByProjectIdAsync(
        long projectId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectFiles
            .AsNoTracking()
            .Where(file => file.ProjectId == projectId)
            .OrderByDescending(file => file.UploadedAt)
            .ThenByDescending(file => file.Id)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(
        ProjectFile file,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectFiles.AddAsync(file, cancellationToken);
}
