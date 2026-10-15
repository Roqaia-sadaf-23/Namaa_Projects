namespace Namaa.Api.Contracts.ProjectMembers;

public sealed record UpdateProjectMemberRequest(
    string? ProjectRole,
    DateTime? LeftAt);
