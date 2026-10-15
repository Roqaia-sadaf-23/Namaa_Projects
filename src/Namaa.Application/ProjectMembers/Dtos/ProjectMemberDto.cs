using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectMembers.Dtos;

public sealed record ProjectMemberDto(
    long Id,
    long ProjectId,
    long UserId,
    string? ProjectRole,
    DateTime JoinedAt,
    DateTime? LeftAt)
{
    internal static ProjectMemberDto FromEntity(ProjectMember member) =>
        new(
            member.Id,
            member.ProjectId,
            member.UserId,
            member.ProjectRole,
            member.JoinedAt,
            member.LeftAt);
}
