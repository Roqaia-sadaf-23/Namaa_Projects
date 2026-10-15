using MediatR;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Abstractions.Time;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Authentication.Commands.Logout;

public sealed class LogoutCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    IUtcClock clock,
    IApplicationDbContext dbContext)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        var refreshToken = await refreshTokenRepository.FindByTokenHashForUpdateAsync(
            tokenHash,
            cancellationToken);

        if (refreshToken is null)
            return Result.Failure(AuthenticationErrors.InvalidRefreshToken);

        if (!refreshToken.RevokedAt.HasValue && refreshToken.ExpiresAt > clock.UtcNow)
        {
            refreshToken.Revoke(clock.UtcNow);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
