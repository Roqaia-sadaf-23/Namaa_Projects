using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectMembers.Queries.GetProjectMemberById;

public sealed class GetProjectMemberByIdQueryHandler(
    IProjectMemberRepository projectMemberRepository)
    : IRequestHandler<GetProjectMemberByIdQuery, Result<ProjectMemberDto>>
{
    public async Task<Result<ProjectMemberDto>> Handle(
        GetProjectMemberByIdQuery request,
        CancellationToken cancellationToken)
    {
        var member = await projectMemberRepository.FindByIdAsync(
            request.ProjectId,
            request.Id,
            cancellationToken);

        return member is null
            ? Result.Failure<ProjectMemberDto>(ProjectMemberErrors.NotFound)
            : Result.Success(ProjectMemberDto.FromEntity(member));
    }
}
