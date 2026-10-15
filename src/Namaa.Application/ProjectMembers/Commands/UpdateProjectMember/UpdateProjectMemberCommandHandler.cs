using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectMembers.Commands.UpdateProjectMember;

public sealed class UpdateProjectMemberCommandHandler(
    IProjectMemberRepository projectMemberRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<UpdateProjectMemberCommand, Result<ProjectMemberDto>>
{
    public async Task<Result<ProjectMemberDto>> Handle(
        UpdateProjectMemberCommand request,
        CancellationToken cancellationToken)
    {
        var member = await projectMemberRepository.FindForUpdateAsync(
            request.ProjectId,
            request.Id,
            cancellationToken);

        if (member is null)
        {
            return Result.Failure<ProjectMemberDto>(ProjectMemberErrors.NotFound);
        }

        if (request.LeftAt.HasValue && request.LeftAt.Value < member.JoinedAt)
        {
            return Result.Failure<ProjectMemberDto>(ProjectMemberErrors.InvalidLeaveDate);
        }

        member.ChangeRole(request.ProjectRole);

        if (request.LeftAt.HasValue)
        {
            member.Leave(request.LeftAt.Value);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectMemberDto.FromEntity(member));
    }
}
