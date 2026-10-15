using MediatR;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectMembers.Queries.GetProjectMembers;

public sealed record GetProjectMembersQuery(
    long ProjectId) : IRequest<Result<IReadOnlyList<ProjectMemberDto>>>;
