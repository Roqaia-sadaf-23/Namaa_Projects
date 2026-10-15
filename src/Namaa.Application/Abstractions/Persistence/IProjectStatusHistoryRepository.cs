namespace Namaa.Application.Abstractions.Persistence;

public interface IProjectStatusHistoryRepository
{
    Task<Namaa.Domain.Entities.ProjectStatusHistory?> FindByIdAsync(long projectId, long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Namaa.Domain.Entities.ProjectStatusHistory>> GetAllAsync(long projectId, CancellationToken cancellationToken = default);
}

