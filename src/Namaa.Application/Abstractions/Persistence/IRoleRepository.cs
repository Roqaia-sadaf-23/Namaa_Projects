using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IRoleRepository
{
    Task<bool> ExistsByNameAsync(string value, byte? excludingId = null, CancellationToken cancellationToken = default);
    Task<Role?> FindByIdAsync(byte id, CancellationToken cancellationToken = default);
    Task<Role?> FindForUpdateAsync(byte id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Role entity, CancellationToken cancellationToken = default);
}

