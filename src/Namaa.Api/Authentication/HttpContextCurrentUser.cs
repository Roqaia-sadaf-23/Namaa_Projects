using System.Security.Claims;
using Namaa.Application.Abstractions.Authentication;

namespace Namaa.Api.Authentication;

public sealed class HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public long? UserId => long.TryParse(
        Principal?.FindFirstValue(ClaimTypes.NameIdentifier),
        out var userId)
            ? userId
            : null;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public IReadOnlyCollection<string> Roles => Principal?
        .FindAll(ClaimTypes.Role)
        .Select(claim => claim.Value)
        .Where(role => !string.IsNullOrWhiteSpace(role))
        .Distinct(StringComparer.Ordinal)
        .ToArray() ?? [];
}
