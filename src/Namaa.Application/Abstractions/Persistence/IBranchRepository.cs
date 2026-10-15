using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IBranchRepository
{
    Task<bool> ExistsByBranchCodeAsync(string value, long? excludingId = null, CancellationToken cancellationToken = default);
    Task<Branch?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Branch?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Branch>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Branch entity, CancellationToken cancellationToken = default);
}

