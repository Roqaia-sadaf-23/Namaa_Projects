namespace Namaa.Domain.Entities;

public class RefreshToken
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public long? ReplacedById { get; set; }

    public User User { get; set; } = null!;
    public RefreshToken? ReplacedBy { get; set; }
    public ICollection<RefreshToken> ReplacementFor { get; set; } = new List<RefreshToken>();
}
