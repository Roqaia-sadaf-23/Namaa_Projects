using MediatR;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectMembers.Commands.AddProjectMember;

public sealed record AddProjectMemberCommand(
    long ProjectId,
    long UserId,
    DateTime JoinedAt,
    string? ProjectRole) : IRequest<Result<ProjectMemberDto>>;
