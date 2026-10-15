using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IServiceRepository
{
    Task<Service?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Service?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Service>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Service entity, CancellationToken cancellationToken = default);
}

