using MediatR;
using Namaa.Application.Branches.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Branches.Queries.GetBranches;

public sealed record GetBranchesQuery : IRequest<Result<IReadOnlyList<BranchDto>>>;

