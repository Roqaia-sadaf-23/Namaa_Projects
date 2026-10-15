using MediatR;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Authentication.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using DomainRefreshToken = Namaa.Domain.Entities.RefreshToken;

namespace Namaa.Application.Authentication.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IApplicationDbContext dbContext)
    : IRequestHandler<LoginCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByEmailWithRoleAsync(
            request.Email.Trim(),
            cancellationToken);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<AuthTokenDto>(AuthenticationErrors.InvalidCredentials);

        if (!user.IsActive)
            return Result.Failure<AuthTokenDto>(AuthenticationErrors.UserInactive);

        var generated = tokenService.GenerateTokens(user, [user.Role.Name]);
        var refreshToken = DomainRefreshToken.Create(
            user.Id,
            generated.RefreshTokenHash,
            generated.RefreshTokenExpiresAt);

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthTokenDto(
            generated.AccessToken,
            generated.RefreshToken,
            generated.AccessTokenExpiresAt,
            generated.RefreshTokenExpiresAt));
    }
}
