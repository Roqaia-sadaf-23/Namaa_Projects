using Microsoft.AspNetCore.Identity;
using Namaa.Application.Abstractions.Authentication;

namespace Namaa.Infrastructure.Authentication;

public sealed class AspNetPasswordHasher : IPasswordHasher
{
    private static readonly object UserContext = new();
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        return _passwordHasher.HashPassword(UserContext, password);
    }

    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(passwordHash))
            return false;

        try
        {
            return _passwordHasher.VerifyHashedPassword(UserContext, passwordHash, password)
                != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
