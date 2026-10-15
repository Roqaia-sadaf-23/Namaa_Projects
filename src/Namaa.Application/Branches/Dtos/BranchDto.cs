using Namaa.Domain.Entities;
using Namaa.Application.Common.Concurrency;

namespace Namaa.Application.Branches.Dtos;

public sealed record BranchDto(long Id, string BranchCode, string Name, string? Address, string? City, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt, string RowVersion)
{
    internal static BranchDto FromEntity(Branch e) => new(e.Id, e.BranchCode, e.Name, e.Address, e.City, e.IsActive, e.CreatedAt, e.UpdatedAt, ConcurrencyToken.Encode(e.RowVersion));
}

