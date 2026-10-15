namespace Namaa.Application.Abstractions.Authentication;

public interface ICurrentUser
{
    long? UserId { get; }

    bool IsAuthenticated { get; }

    IReadOnlyCollection<string> Roles { get; }
}
