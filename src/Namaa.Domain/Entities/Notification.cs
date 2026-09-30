namespace Namaa.Domain.Entities;

public class Notification
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public long? EntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public User User { get; set; } = null!;
}
