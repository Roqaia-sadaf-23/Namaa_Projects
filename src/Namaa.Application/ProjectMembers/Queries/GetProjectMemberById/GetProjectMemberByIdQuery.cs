using MediatR;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectMembers.Queries.GetProjectMemberById;

public sealed record GetProjectMemberByIdQuery(
    long ProjectId,
    long Id) : IRequest<Result<ProjectMemberDto>>;
