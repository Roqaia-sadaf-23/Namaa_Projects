using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Authentication;

public interface ITokenService
{
    TokenResult GenerateTokens(User user, IReadOnlyCollection<string> roles);

    string HashRefreshToken(string refreshToken);
}

public sealed record TokenResult(
    string AccessToken,
    string RefreshToken,
    string RefreshTokenHash,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
