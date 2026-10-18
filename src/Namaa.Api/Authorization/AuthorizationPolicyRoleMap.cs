using Namaa.Application.Common.Authorization;

namespace Namaa.Api.Authorization;

public static class AuthorizationPolicyRoleMap
{
    public static IReadOnlyList<string> PolicyNames { get; } =
    [
        AuthorizationPolicies.CanManageProjects,
        AuthorizationPolicies.CanManageCrmSales,
        AuthorizationPolicies.CanManageFinance,
        AuthorizationPolicies.CanPostJournalEntries,
        AuthorizationPolicies.CanManageUsers,
        AuthorizationPolicies.CanManageRoles,
        AuthorizationPolicies.CanManageProcurement,
        AuthorizationPolicies.CanViewNotifications,
        AuthorizationPolicies.CanViewAuditLogs
    ];

    public static string[] GetAllowedRoles(string policyName) => policyName switch
    {
        AuthorizationPolicies.CanManageProjects =>
            [RoleNames.Administrator, RoleNames.ProjectManager],
        AuthorizationPolicies.CanManageCrmSales =>
            [RoleNames.Administrator, RoleNames.Sales],
        AuthorizationPolicies.CanManageFinance =>
            [RoleNames.Administrator, RoleNames.Accountant],
        AuthorizationPolicies.CanPostJournalEntries =>
            [RoleNames.Administrator, RoleNames.Accountant],
        AuthorizationPolicies.CanManageUsers =>
            [RoleNames.Administrator, RoleNames.HR],
        AuthorizationPolicies.CanManageRoles =>
            [RoleNames.Administrator],
        AuthorizationPolicies.CanManageProcurement =>
            [RoleNames.Administrator, RoleNames.Procurement],
        AuthorizationPolicies.CanViewNotifications => [],
        AuthorizationPolicies.CanViewAuditLogs =>
            [RoleNames.Administrator],
        _ => throw new ArgumentOutOfRangeException(
            nameof(policyName),
            policyName,
            "The authorization policy is not registered in the role map.")
    };
}
