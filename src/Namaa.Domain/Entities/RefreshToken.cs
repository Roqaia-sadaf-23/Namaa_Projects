namespace Namaa.Domain.Entities;

public class RefreshToken
{
    private RefreshToken()
    {
    }

    private RefreshToken(long userId, string tokenHash, DateTime expiresAt, DateTime createdAt)
    {
        UserId = DomainGuard.Positive(userId, nameof(userId));
        TokenHash = DomainGuard.Required(tokenHash, nameof(tokenHash));
        if (expiresAt <= createdAt)
            throw new ArgumentException("Expiration must be after token creation.", nameof(expiresAt));

        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public long? ReplacedById { get; private set; }

    public User User { get; private set; } = null!;
    public RefreshToken? ReplacedBy { get; private set; }
    public ICollection<RefreshToken> ReplacementFor { get; private set; } = new List<RefreshToken>();

    public static RefreshToken Create(long userId, string tokenHash, DateTime expiresAt)
    {
        var createdAt = DateTime.UtcNow;
        return new RefreshToken(userId, tokenHash, expiresAt, createdAt);
    }

    public void Revoke(DateTime? revokedAt = null, long? replacedById = null)
    {
        var effectiveRevokedAt = revokedAt ?? DateTime.UtcNow;
        if (effectiveRevokedAt < CreatedAt)
            throw new ArgumentException("Revocation cannot be before token creation.", nameof(revokedAt));

        DomainGuard.OptionalPositive(replacedById, nameof(replacedById));
        RevokedAt = effectiveRevokedAt;
        ReplacedById = replacedById;
    }
}
