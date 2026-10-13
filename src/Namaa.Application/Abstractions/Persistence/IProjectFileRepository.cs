using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IProjectFileRepository
{
    Task<bool> ExistsByStorageKeyAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    Task<ProjectFile?> FindByIdAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default);

    Task<ProjectFile?> FindForUpdateAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectFile>> GetByProjectIdAsync(
        long projectId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProjectFile file,
        CancellationToken cancellationToken = default);
}
