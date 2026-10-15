using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IProjectCorrespondenceRepository
{
    Task<ProjectCorrespondence?> FindByIdAsync(long projectId, long id, CancellationToken cancellationToken = default);
    Task<ProjectCorrespondence?> FindForUpdateAsync(long projectId, long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProjectCorrespondence>> GetAllAsync(long projectId, CancellationToken cancellationToken = default);
    Task AddAsync(ProjectCorrespondence correspondence, CancellationToken cancellationToken = default);
}

