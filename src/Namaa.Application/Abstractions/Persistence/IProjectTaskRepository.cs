using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IProjectTaskRepository
{
    Task<ProjectTask?> FindByIdAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default);

    Task<ProjectTask?> FindForUpdateAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectTask>> GetByProjectIdAsync(
        long projectId,
        long? stageId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProjectTask task,
        CancellationToken cancellationToken = default);
}
