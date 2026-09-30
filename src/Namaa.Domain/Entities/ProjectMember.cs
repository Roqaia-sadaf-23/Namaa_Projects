namespace Namaa.Domain.Entities;

public class ProjectMember
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public long UserId { get; set; }
    public string? ProjectRole { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }

    public Project Project { get; set; } = null!;
    public User User { get; set; } = null!;
}
