namespace Namaa.Api.Contracts.CostCenters;public sealed record UpdateCostCenterRequest(string Name,string? Description,long? ParentCostCenterId,long? BranchId,string RowVersion);
