using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IProjectStageRepository
{
    Task<bool> ExistsByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<ProjectStage?> FindByIdAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default);

    Task<ProjectStage?> FindForUpdateAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectStage>> GetByProjectIdAsync(
        long projectId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProjectStage stage,
        CancellationToken cancellationToken = default);
}
