using MediatR;
using Namaa.Application.Branches.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Branches.Commands.UpdateBranch;

public sealed record UpdateBranchCommand(long Id, string Name, string? Address, string? City) : IRequest<Result<BranchDto>>;

