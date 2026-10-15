using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    Task<bool> ExistsByCodeAsync(
        string customerCode,
        CancellationToken cancellationToken = default);

    Task<Customer?> FindByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Customer?> FindForUpdateAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Customer?> FindByIdWithContactsAsync(long id, CancellationToken cancellationToken = default);
    Task<Customer?> FindForUpdateWithContactsAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Customer>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default);
}
