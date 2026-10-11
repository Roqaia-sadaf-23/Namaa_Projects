namespace Namaa.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<bool> ExistsByIdAsync(
        long id,
        CancellationToken cancellationToken = default);
}
