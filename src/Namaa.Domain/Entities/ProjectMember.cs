namespace Namaa.Domain.Entities;

public class ProjectMember
{
    private ProjectMember()
    {
    }

    private ProjectMember(long projectId, long userId, DateTime joinedAt, string? projectRole)
    {
        ProjectId = DomainGuard.Positive(projectId, nameof(projectId));
        UserId = DomainGuard.Positive(userId, nameof(userId));
        JoinedAt = joinedAt;
        ProjectRole = projectRole;
    }

    public long Id { get; private set; }
    public long ProjectId { get; private set; }
    public long UserId { get; private set; }
    public string? ProjectRole { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public DateTime? LeftAt { get; private set; }

    public Project Project { get; private set; } = null!;
    public User User { get; private set; } = null!;

    public static ProjectMember Create(long projectId, long userId, DateTime joinedAt, string? projectRole = null) =>
        new(projectId, userId, joinedAt, projectRole);

    public void ChangeRole(string? projectRole) => ProjectRole = projectRole;

    public void Leave(DateTime leftAt)
    {
        DomainGuard.EndNotBeforeStart(JoinedAt, leftAt, nameof(leftAt));
        LeftAt = leftAt;
    }
}
