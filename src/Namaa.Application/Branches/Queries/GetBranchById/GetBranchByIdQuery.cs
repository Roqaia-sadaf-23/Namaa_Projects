using MediatR;
using Namaa.Application.Branches.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Branches.Queries.GetBranchById;

public sealed record GetBranchByIdQuery(long Id) : IRequest<Result<BranchDto>>;

