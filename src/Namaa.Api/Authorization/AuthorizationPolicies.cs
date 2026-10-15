namespace Namaa.Api.Authorization;

public static class AuthorizationPolicies
{
    public const string CanManageProjects = nameof(CanManageProjects);
    public const string CanManageCrmSales = nameof(CanManageCrmSales);
    public const string CanManageFinance = nameof(CanManageFinance);
    public const string CanPostJournalEntries = nameof(CanPostJournalEntries);
    public const string CanManageUsers = nameof(CanManageUsers);
    public const string CanManageRoles = nameof(CanManageRoles);
    public const string CanManageProcurement = nameof(CanManageProcurement);
    public const string CanViewNotifications = nameof(CanViewNotifications);
    public const string CanViewAuditLogs = nameof(CanViewAuditLogs);
}
