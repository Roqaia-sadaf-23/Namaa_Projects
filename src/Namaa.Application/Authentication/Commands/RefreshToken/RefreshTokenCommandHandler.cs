using MediatR;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Abstractions.Time;
using Namaa.Application.Authentication.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using DomainRefreshToken = Namaa.Domain.Entities.RefreshToken;

namespace Namaa.Application.Authentication.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    IUtcClock clock,
    IApplicationDbContext dbContext)
    : IRequestHandler<RefreshTokenCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        var currentToken = await refreshTokenRepository.FindByTokenHashForUpdateAsync(
            tokenHash,
            cancellationToken);

        if (currentToken is null)
            return Result.Failure<AuthTokenDto>(AuthenticationErrors.InvalidRefreshToken);

        if (currentToken.RevokedAt.HasValue)
            return Result.Failure<AuthTokenDto>(AuthenticationErrors.RevokedRefreshToken);

        if (currentToken.ExpiresAt <= clock.UtcNow)
            return Result.Failure<AuthTokenDto>(AuthenticationErrors.ExpiredRefreshToken);

        if (!currentToken.User.IsActive)
            return Result.Failure<AuthTokenDto>(AuthenticationErrors.UserInactive);

        var generated = tokenService.GenerateTokens(
            currentToken.User,
            [currentToken.User.Role.Name]);
        var replacement = DomainRefreshToken.Create(
            currentToken.UserId,
            generated.RefreshTokenHash,
            generated.RefreshTokenExpiresAt);

        currentToken.RevokeAndReplace(replacement, clock.UtcNow);
        await refreshTokenRepository.AddAsync(replacement, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthTokenDto(
            generated.AccessToken,
            generated.RefreshToken,
            generated.AccessTokenExpiresAt,
            generated.RefreshTokenExpiresAt));
    }
}
