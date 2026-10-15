namespace Namaa.Api.Contracts.ProjectMembers;

public sealed record AddProjectMemberRequest(
    long UserId,
    DateTime JoinedAt,
    string? ProjectRole);
