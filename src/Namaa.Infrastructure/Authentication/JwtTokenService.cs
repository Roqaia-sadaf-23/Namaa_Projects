using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Time;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Authentication;

public sealed class JwtTokenService(IOptions<JwtOptions> options, IUtcClock clock) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public TokenResult GenerateTokens(User user, IReadOnlyCollection<string> roles)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);

        var issuedAt = clock.UtcNow;
        var accessTokenExpiresAt = issuedAt.AddMinutes(_options.AccessTokenMinutes);
        var refreshTokenExpiresAt = issuedAt.AddDays(_options.RefreshTokenDays);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

        claims.AddRange(roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.Ordinal)
            .Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAt,
            expires: accessTokenExpiresAt,
            signingCredentials: credentials);

        var refreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));

        return new TokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken,
            HashRefreshToken(refreshToken),
            accessTokenExpiresAt,
            refreshTokenExpiresAt);
    }

    public string HashRefreshToken(string refreshToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }
}
