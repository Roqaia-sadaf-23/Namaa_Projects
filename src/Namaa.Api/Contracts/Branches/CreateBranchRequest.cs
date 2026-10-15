namespace Namaa.Api.Contracts.Branches;

public sealed record CreateBranchRequest(string BranchCode, string Name, string? Address, string? City);

