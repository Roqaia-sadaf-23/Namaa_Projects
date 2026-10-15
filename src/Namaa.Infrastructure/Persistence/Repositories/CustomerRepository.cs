using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(NamaaDbContext dbContext) : ICustomerRepository
{
    public Task<bool> ExistsByCodeAsync(
        string customerCode,
        CancellationToken cancellationToken = default) =>
        dbContext.Customers
            .AsNoTracking()
            .AnyAsync(
                customer => customer.CustomerCode == customerCode,
                cancellationToken);

    public Task<Customer?> FindByIdAsync(
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(customer => customer.Id == id, cancellationToken);

    public Task<Customer?> FindForUpdateAsync(
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.Customers
            .SingleOrDefaultAsync(customer => customer.Id == id, cancellationToken);

    public Task<Customer?> FindByIdWithContactsAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Customers.AsNoTracking().Include(customer => customer.Contacts).SingleOrDefaultAsync(customer => customer.Id == id, cancellationToken);

    public Task<Customer?> FindForUpdateWithContactsAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Customers.Include(customer => customer.Contacts).SingleOrDefaultAsync(customer => customer.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Customer>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.Id)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default) =>
        await dbContext.Customers.AddAsync(customer, cancellationToken);
}
