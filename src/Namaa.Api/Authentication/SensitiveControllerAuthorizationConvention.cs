using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Namaa.Api.Authentication;

public sealed class SensitiveControllerAuthorizationConvention : IControllerModelConvention
{
    private static readonly HashSet<string> ProtectedControllers = new(StringComparer.Ordinal)
    {
        "Accounts",
        "AuditLogs",
        "BusinessWorkflows",
        "CostCenters",
        "Expenses",
        "Invoices",
        "JournalEntries",
        "Notifications",
        "Payments",
        "PaymentVouchers",
        "PurchaseInvoices",
        "Users"
    };

    public void Apply(ControllerModel controller)
    {
        if (ProtectedControllers.Contains(controller.ControllerName))
            controller.Filters.Add(new AuthorizeFilter(new[] { new AuthorizeAttribute() }));
    }
}
