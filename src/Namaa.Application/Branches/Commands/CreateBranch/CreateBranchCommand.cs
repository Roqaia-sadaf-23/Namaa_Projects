using MediatR;
using Namaa.Application.Branches.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Branches.Commands.CreateBranch;

public sealed record CreateBranchCommand(string BranchCode, string Name, string? Address, string? City) : IRequest<Result<BranchDto>>;

