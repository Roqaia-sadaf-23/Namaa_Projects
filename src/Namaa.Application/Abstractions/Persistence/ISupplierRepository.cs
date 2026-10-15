using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface ISupplierRepository
{
    Task<bool> ExistsBySupplierCodeAsync(string value, long? excludingId = null, CancellationToken cancellationToken = default);
    Task<Supplier?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Supplier?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Supplier entity, CancellationToken cancellationToken = default);
}

