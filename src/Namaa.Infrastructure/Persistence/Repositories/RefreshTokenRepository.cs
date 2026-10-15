using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository(NamaaDbContext dbContext) : IRefreshTokenRepository
{
    public Task<RefreshToken?> FindByTokenHashForUpdateAsync(
        string tokenHash,
        CancellationToken cancellationToken = default) =>
        dbContext.RefreshTokens
            .Include(token => token.User)
            .ThenInclude(user => user.Role)
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default) =>
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
}
