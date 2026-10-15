namespace Namaa.Application.Authentication.Dtos;

public sealed record AuthTokenDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);
