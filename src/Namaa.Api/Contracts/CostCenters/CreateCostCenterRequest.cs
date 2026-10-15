namespace Namaa.Api.Contracts.CostCenters;public sealed record CreateCostCenterRequest(string CostCenterCode,string Name,string? Description,long? ParentCostCenterId,long? BranchId);
