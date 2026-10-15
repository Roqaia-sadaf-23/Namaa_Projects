using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectMembers.Commands.AddProjectMember;

public sealed class AddProjectMemberCommandHandler(
    IProjectMemberRepository projectMemberRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<AddProjectMemberCommand, Result<ProjectMemberDto>>
{
    public async Task<Result<ProjectMemberDto>> Handle(
        AddProjectMemberCommand request,
        CancellationToken cancellationToken)
    {
        if (await projectRepository.FindByIdAsync(
                request.ProjectId,
                cancellationToken) is null)
        {
            return Result.Failure<ProjectMemberDto>(ProjectMemberErrors.ProjectNotFound);
        }

        if (!await userRepository.ExistsByIdAsync(
                request.UserId,
                cancellationToken))
        {
            return Result.Failure<ProjectMemberDto>(ProjectMemberErrors.UserNotFound);
        }

        if (await projectMemberRepository.ExistsAsync(
                request.ProjectId,
                request.UserId,
                cancellationToken))
        {
            return Result.Failure<ProjectMemberDto>(ProjectMemberErrors.DuplicateMember);
        }

        var member = ProjectMember.Create(
            request.ProjectId,
            request.UserId,
            request.JoinedAt,
            request.ProjectRole);

        await projectMemberRepository.AddAsync(member, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectMemberDto.FromEntity(member));
    }
}
