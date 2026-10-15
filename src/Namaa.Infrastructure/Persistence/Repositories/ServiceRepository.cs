using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class ServiceRepository(NamaaDbContext dbContext) : IServiceRepository
{
    public Task<Service?> FindByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Services.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Service?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Services.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Service>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Services.AsNoTracking().OrderBy(e => e.Id).ToListAsync(cancellationToken);

    public async Task AddAsync(Service entity, CancellationToken cancellationToken = default) =>
        await dbContext.Services.AddAsync(entity, cancellationToken);
}

