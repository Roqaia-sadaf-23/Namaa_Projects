using MediatR;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectMembers.Commands.UpdateProjectMember;

public sealed record UpdateProjectMemberCommand(
    long ProjectId,
    long Id,
    string? ProjectRole,
    DateTime? LeftAt) : IRequest<Result<ProjectMemberDto>>;
