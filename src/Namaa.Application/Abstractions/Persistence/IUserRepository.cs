using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<bool> ExistsByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<User?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<User?> FindByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, long excludingId, CancellationToken cancellationToken = default);
    Task<bool> PhoneExistsAsync(string phoneNumber, long excludingId, CancellationToken cancellationToken = default);
}
