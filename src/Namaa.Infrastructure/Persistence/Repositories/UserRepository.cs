using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(NamaaDbContext dbContext) : IUserRepository
{
    public Task<bool> ExistsByIdAsync(
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == id, cancellationToken);
}
