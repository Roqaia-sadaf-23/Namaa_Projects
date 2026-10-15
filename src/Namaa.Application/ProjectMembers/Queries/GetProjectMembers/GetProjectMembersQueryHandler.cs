using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectMembers.Queries.GetProjectMembers;

public sealed class GetProjectMembersQueryHandler(
    IProjectMemberRepository projectMemberRepository,
    IProjectRepository projectRepository)
    : IRequestHandler<GetProjectMembersQuery, Result<IReadOnlyList<ProjectMemberDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectMemberDto>>> Handle(
        GetProjectMembersQuery request,
        CancellationToken cancellationToken)
    {
        if (await projectRepository.FindByIdAsync(
                request.ProjectId,
                cancellationToken) is null)
        {
            return Result.Failure<IReadOnlyList<ProjectMemberDto>>(
                ProjectMemberErrors.ProjectNotFound);
        }

        var members = await projectMemberRepository.GetByProjectIdAsync(
            request.ProjectId,
            cancellationToken);

        return Result.Success<IReadOnlyList<ProjectMemberDto>>(
            members
                .Select(ProjectMemberDto.FromEntity)
                .ToArray());
    }
}
