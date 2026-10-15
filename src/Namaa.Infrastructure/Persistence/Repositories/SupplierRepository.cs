using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class SupplierRepository(NamaaDbContext dbContext) : ISupplierRepository
{
    public Task<bool> ExistsBySupplierCodeAsync(string value, long? excludingId = null, CancellationToken cancellationToken = default) =>
        dbContext.Suppliers.AsNoTracking().AnyAsync(e => e.SupplierCode == value && (!excludingId.HasValue || e.Id != excludingId.Value), cancellationToken);

    public Task<Supplier?> FindByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Suppliers.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Supplier?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Suppliers.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Suppliers.AsNoTracking().OrderBy(e => e.Id).ToListAsync(cancellationToken);

    public async Task AddAsync(Supplier entity, CancellationToken cancellationToken = default) =>
        await dbContext.Suppliers.AddAsync(entity, cancellationToken);
}

