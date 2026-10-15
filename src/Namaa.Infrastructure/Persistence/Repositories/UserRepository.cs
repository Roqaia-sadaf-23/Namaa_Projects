using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(NamaaDbContext dbContext) : IUserRepository
{
    public Task<bool> ExistsByIdAsync(
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == id, cancellationToken);

    public Task<User?> FindByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == id, cancellationToken);

    public Task<User?> FindByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default) =>
        dbContext.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

    public Task<User?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Users.AsNoTracking().OrderBy(user => user.Id).ToListAsync(cancellationToken);

    public Task<bool> EmailExistsAsync(string email, long excludingId, CancellationToken cancellationToken = default) =>
        dbContext.Users.AsNoTracking().AnyAsync(user => user.Email == email && user.Id != excludingId, cancellationToken);

    public Task<bool> PhoneExistsAsync(string phoneNumber, long excludingId, CancellationToken cancellationToken = default) =>
        dbContext.Users.AsNoTracking().AnyAsync(user => user.PhoneNumber == phoneNumber && user.Id != excludingId, cancellationToken);
}
