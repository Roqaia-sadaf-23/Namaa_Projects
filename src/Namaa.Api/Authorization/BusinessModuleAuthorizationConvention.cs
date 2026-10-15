using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Namaa.Api.Authorization;

public sealed class BusinessModuleAuthorizationConvention : IControllerModelConvention
{
    private static readonly Dictionary<string, string> ControllerPolicies =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Projects"] = AuthorizationPolicies.CanManageProjects,
            ["ProjectStages"] = AuthorizationPolicies.CanManageProjects,
            ["ProjectTasks"] = AuthorizationPolicies.CanManageProjects,
            ["ProjectFiles"] = AuthorizationPolicies.CanManageProjects,
            ["ProjectMembers"] = AuthorizationPolicies.CanManageProjects,
            ["ProjectCorrespondences"] = AuthorizationPolicies.CanManageProjects,
            ["ProjectStatusHistory"] = AuthorizationPolicies.CanManageProjects,
            ["Customers"] = AuthorizationPolicies.CanManageCrmSales,
            ["CustomerContacts"] = AuthorizationPolicies.CanManageCrmSales,
            ["Services"] = AuthorizationPolicies.CanManageCrmSales,
            ["Quotations"] = AuthorizationPolicies.CanManageCrmSales,
            ["Accounts"] = AuthorizationPolicies.CanManageFinance,
            ["Invoices"] = AuthorizationPolicies.CanManageFinance,
            ["Payments"] = AuthorizationPolicies.CanManageFinance,
            ["PaymentVouchers"] = AuthorizationPolicies.CanManageFinance,
            ["Expenses"] = AuthorizationPolicies.CanManageFinance,
            ["JournalEntries"] = AuthorizationPolicies.CanManageFinance,
            ["CostCenters"] = AuthorizationPolicies.CanManageFinance,
            ["Users"] = AuthorizationPolicies.CanManageUsers,
            ["Roles"] = AuthorizationPolicies.CanManageRoles,
            ["Suppliers"] = AuthorizationPolicies.CanManageProcurement,
            ["PurchaseInvoices"] = AuthorizationPolicies.CanManageProcurement,
            ["Branches"] = AuthorizationPolicies.CanManageProcurement,
            ["Notifications"] = AuthorizationPolicies.CanViewNotifications,
            ["AuditLogs"] = AuthorizationPolicies.CanViewAuditLogs
        };

    public void Apply(ControllerModel controller)
    {
        if (ControllerPolicies.TryGetValue(controller.ControllerName, out var policy))
            controller.Filters.Add(CreateFilter(policy));

        if (controller.ControllerName == "JournalEntries")
        {
            AddActionPolicy(
                controller,
                "Post",
                AuthorizationPolicies.CanPostJournalEntries);
        }

        if (controller.ControllerName == "BusinessWorkflows")
            ApplyWorkflowPolicies(controller);
    }

    private static void ApplyWorkflowPolicies(ControllerModel controller)
    {
        foreach (var action in controller.Actions)
        {
            var policy = action.ActionName switch
            {
                "SendQuotation" or "ApproveQuotation" or "RejectQuotation" or "CancelQuotation" =>
                    AuthorizationPolicies.CanManageCrmSales,
                "IssueInvoice" or "CancelInvoice" or "PostPayment" or "CancelPayment" or
                    "PostVoucher" or "CancelVoucher" => AuthorizationPolicies.CanManageFinance,
                "PostPurchaseInvoice" or "CancelPurchaseInvoice" =>
                    AuthorizationPolicies.CanManageProcurement,
                _ => throw new InvalidOperationException(
                    $"No authorization policy is configured for workflow action '{action.ActionName}'.")
            };

            action.Filters.Add(CreateFilter(policy));
        }
    }

    private static void AddActionPolicy(
        ControllerModel controller,
        string actionName,
        string policy)
    {
        var action = controller.Actions.Single(action => action.ActionName == actionName);
        action.Filters.Add(CreateFilter(policy));
    }

    private static AuthorizeFilter CreateFilter(string policy) =>
        new(new[] { new AuthorizeAttribute { Policy = policy } });
}
