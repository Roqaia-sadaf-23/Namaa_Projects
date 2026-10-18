using System.Text.Json;
using Namaa.Application.Accounts.Commands.CreateAccount;
using Namaa.Application.Accounts.Commands.UpdateAccount;
using Namaa.Application.Branches.Commands.CreateBranch;
using Namaa.Application.Branches.Commands.UpdateBranch;
using Namaa.Application.CostCenters.Commands.CreateCostCenter;
using Namaa.Application.CostCenters.Commands.UpdateCostCenter;
using Namaa.Application.CustomerContacts.Commands.AddCustomerContact;
using Namaa.Application.CustomerContacts.Commands.UpdateCustomerContact;
using Namaa.Application.Customers.Commands.CreateCustomer;
using Namaa.Application.Customers.Commands.UpdateCustomer;
using Namaa.Application.Expenses.Commands.CreateExpense;
using Namaa.Application.Expenses.Commands.UpdateExpense;
using Namaa.Application.Invoices.Commands.AddInvoiceItem;
using Namaa.Application.Invoices.Commands.CancelInvoice;
using Namaa.Application.Invoices.Commands.CreateInvoice;
using Namaa.Application.Invoices.Commands.IssueInvoice;
using Namaa.Application.Invoices.Commands.UpdateInvoice;
using Namaa.Application.JournalEntries.Commands.AddJournalEntryLine;
using Namaa.Application.JournalEntries.Commands.CreateJournalEntry;
using Namaa.Application.JournalEntries.Commands.PostJournalEntry;
using Namaa.Application.Notifications.Commands.MarkNotificationRead;
using Namaa.Application.Payments.Commands.CancelPayment;
using Namaa.Application.Payments.Commands.CreatePayment;
using Namaa.Application.Payments.Commands.PostPayment;
using Namaa.Application.Payments.Commands.UpdatePayment;
using Namaa.Application.PaymentVouchers.Commands.AddPaymentVoucherAllocation;
using Namaa.Application.PaymentVouchers.Commands.CancelPaymentVoucher;
using Namaa.Application.PaymentVouchers.Commands.CreatePaymentVoucher;
using Namaa.Application.PaymentVouchers.Commands.PostPaymentVoucher;
using Namaa.Application.ProjectCorrespondences.Commands.CreateProjectCorrespondence;
using Namaa.Application.ProjectCorrespondences.Commands.UpdateProjectCorrespondence;
using Namaa.Application.ProjectFiles.Commands.CreateProjectFile;
using Namaa.Application.ProjectFiles.Commands.UpdateProjectFile;
using Namaa.Application.ProjectMembers.Commands.AddProjectMember;
using Namaa.Application.ProjectMembers.Commands.UpdateProjectMember;
using Namaa.Application.Projects.Commands.ChangeProjectStatus;
using Namaa.Application.Projects.Commands.CreateProject;
using Namaa.Application.Projects.Commands.UpdateProject;
using Namaa.Application.ProjectStages.Commands.CreateProjectStage;
using Namaa.Application.ProjectStages.Commands.UpdateProjectStage;
using Namaa.Application.ProjectTasks.Commands.CreateProjectTask;
using Namaa.Application.ProjectTasks.Commands.UpdateProjectTask;
using Namaa.Application.PurchaseInvoices.Commands.AddPurchaseInvoiceItem;
using Namaa.Application.PurchaseInvoices.Commands.CancelPurchaseInvoice;
using Namaa.Application.PurchaseInvoices.Commands.CreatePurchaseInvoice;
using Namaa.Application.PurchaseInvoices.Commands.PostPurchaseInvoice;
using Namaa.Application.PurchaseInvoices.Commands.UpdatePurchaseInvoice;
using Namaa.Application.Quotations.Commands.AddQuotationItem;
using Namaa.Application.Quotations.Commands.AddQuotationPaymentTerm;
using Namaa.Application.Quotations.Commands.ApproveQuotation;
using Namaa.Application.Quotations.Commands.CancelQuotation;
using Namaa.Application.Quotations.Commands.CreateQuotation;
using Namaa.Application.Quotations.Commands.RejectQuotation;
using Namaa.Application.Quotations.Commands.SendQuotation;
using Namaa.Application.Quotations.Commands.UpdateQuotation;
using Namaa.Application.Roles.Commands.CreateRole;
using Namaa.Application.Roles.Commands.UpdateRole;
using Namaa.Application.Services.Commands.CreateService;
using Namaa.Application.Services.Commands.UpdateService;
using Namaa.Application.Suppliers.Commands.CreateSupplier;
using Namaa.Application.Suppliers.Commands.UpdateSupplier;
using Namaa.Application.Users.Commands.UpdateUser;

namespace Namaa.Application.Common.Auditing;

internal static class AuditCommandCatalog
{
    private static readonly Dictionary<Type, AuditDefinition> Definitions =
        new Dictionary<Type, AuditDefinition>
        {
            [typeof(CreateAccountCommand)] = Define(AuditEntities.Account, AuditActions.Created),
            [typeof(UpdateAccountCommand)] = Define(AuditEntities.Account, AuditActions.Updated),
            [typeof(CreateBranchCommand)] = Define(AuditEntities.Branch, AuditActions.Created),
            [typeof(UpdateBranchCommand)] = Define(AuditEntities.Branch, AuditActions.Updated),
            [typeof(CreateCostCenterCommand)] = Define(AuditEntities.CostCenter, AuditActions.Created),
            [typeof(UpdateCostCenterCommand)] = Define(AuditEntities.CostCenter, AuditActions.Updated),
            [typeof(AddCustomerContactCommand)] = Define(AuditEntities.CustomerContact, AuditActions.Added),
            [typeof(UpdateCustomerContactCommand)] = Define(AuditEntities.CustomerContact, AuditActions.Updated),
            [typeof(CreateCustomerCommand)] = Define(AuditEntities.Customer, AuditActions.Created),
            [typeof(UpdateCustomerCommand)] = Define(AuditEntities.Customer, AuditActions.Updated),
            [typeof(CreateExpenseCommand)] = Define(AuditEntities.Expense, AuditActions.Created),
            [typeof(UpdateExpenseCommand)] = Define(AuditEntities.Expense, AuditActions.Updated),
            [typeof(AddInvoiceItemCommand)] = Define(AuditEntities.InvoiceItem, AuditActions.Added),
            [typeof(CancelInvoiceCommand)] = Define(AuditEntities.Invoice, AuditActions.Cancelled),
            [typeof(CreateInvoiceCommand)] = Define(AuditEntities.Invoice, AuditActions.Created),
            [typeof(IssueInvoiceCommand)] = Define(AuditEntities.Invoice, AuditActions.Issued),
            [typeof(UpdateInvoiceCommand)] = Define(AuditEntities.Invoice, AuditActions.Updated),
            [typeof(AddJournalEntryLineCommand)] = Define(AuditEntities.JournalEntryLine, AuditActions.Added),
            [typeof(CreateJournalEntryCommand)] = Define(AuditEntities.JournalEntry, AuditActions.Created),
            [typeof(PostJournalEntryCommand)] = Define(AuditEntities.JournalEntry, AuditActions.Posted),
            [typeof(MarkNotificationReadCommand)] = Define(AuditEntities.Notification, AuditActions.ReadStateChanged),
            [typeof(CancelPaymentCommand)] = Define(AuditEntities.Payment, AuditActions.Cancelled),
            [typeof(CreatePaymentCommand)] = Define(AuditEntities.Payment, AuditActions.Created),
            [typeof(PostPaymentCommand)] = Define(AuditEntities.Payment, AuditActions.Posted),
            [typeof(UpdatePaymentCommand)] = Define(AuditEntities.Payment, AuditActions.Updated),
            [typeof(AddPaymentVoucherAllocationCommand)] = Define(
                AuditEntities.PaymentVoucherAllocation,
                AuditActions.Added),
            [typeof(CancelPaymentVoucherCommand)] = Define(AuditEntities.PaymentVoucher, AuditActions.Cancelled),
            [typeof(CreatePaymentVoucherCommand)] = Define(AuditEntities.PaymentVoucher, AuditActions.Created),
            [typeof(PostPaymentVoucherCommand)] = Define(AuditEntities.PaymentVoucher, AuditActions.Posted),
            [typeof(CreateProjectCorrespondenceCommand)] = Define(
                AuditEntities.ProjectCorrespondence,
                AuditActions.Created),
            [typeof(UpdateProjectCorrespondenceCommand)] = Define(
                AuditEntities.ProjectCorrespondence,
                AuditActions.Updated),
            [typeof(CreateProjectFileCommand)] = Define(AuditEntities.ProjectFile, AuditActions.MetadataCreated),
            [typeof(UpdateProjectFileCommand)] = Define(AuditEntities.ProjectFile, AuditActions.MetadataUpdated),
            [typeof(AddProjectMemberCommand)] = Define(AuditEntities.ProjectMember, AuditActions.Added),
            [typeof(UpdateProjectMemberCommand)] = Define(AuditEntities.ProjectMember, AuditActions.Updated),
            [typeof(ChangeProjectStatusCommand)] = Define(
                AuditEntities.Project,
                AuditActions.StatusChanged,
                request => JsonSerializer.Serialize(new
                {
                    status = ((ChangeProjectStatusCommand)request).Status.ToString()
                })),
            [typeof(CreateProjectCommand)] = Define(AuditEntities.Project, AuditActions.Created),
            [typeof(UpdateProjectCommand)] = Define(AuditEntities.Project, AuditActions.Updated),
            [typeof(CreateProjectStageCommand)] = Define(AuditEntities.ProjectStage, AuditActions.Created),
            [typeof(UpdateProjectStageCommand)] = Define(AuditEntities.ProjectStage, AuditActions.Updated),
            [typeof(CreateProjectTaskCommand)] = Define(AuditEntities.ProjectTask, AuditActions.Created),
            [typeof(UpdateProjectTaskCommand)] = Define(AuditEntities.ProjectTask, AuditActions.Updated),
            [typeof(AddPurchaseInvoiceItemCommand)] = Define(AuditEntities.PurchaseInvoiceItem, AuditActions.Added),
            [typeof(CancelPurchaseInvoiceCommand)] = Define(AuditEntities.PurchaseInvoice, AuditActions.Cancelled),
            [typeof(CreatePurchaseInvoiceCommand)] = Define(AuditEntities.PurchaseInvoice, AuditActions.Created),
            [typeof(PostPurchaseInvoiceCommand)] = Define(AuditEntities.PurchaseInvoice, AuditActions.Posted),
            [typeof(UpdatePurchaseInvoiceCommand)] = Define(AuditEntities.PurchaseInvoice, AuditActions.Updated),
            [typeof(AddQuotationItemCommand)] = Define(AuditEntities.QuotationItem, AuditActions.Added),
            [typeof(AddQuotationPaymentTermCommand)] = Define(
                AuditEntities.QuotationPaymentTerm,
                AuditActions.Added),
            [typeof(ApproveQuotationCommand)] = Define(AuditEntities.Quotation, AuditActions.Approved),
            [typeof(CancelQuotationCommand)] = Define(AuditEntities.Quotation, AuditActions.Cancelled),
            [typeof(CreateQuotationCommand)] = Define(AuditEntities.Quotation, AuditActions.Created),
            [typeof(RejectQuotationCommand)] = Define(AuditEntities.Quotation, AuditActions.Rejected),
            [typeof(SendQuotationCommand)] = Define(AuditEntities.Quotation, AuditActions.Sent),
            [typeof(UpdateQuotationCommand)] = Define(AuditEntities.Quotation, AuditActions.Updated),
            [typeof(CreateRoleCommand)] = Define(AuditEntities.Role, AuditActions.Created),
            [typeof(UpdateRoleCommand)] = Define(AuditEntities.Role, AuditActions.Updated),
            [typeof(CreateServiceCommand)] = Define(AuditEntities.Service, AuditActions.Created),
            [typeof(UpdateServiceCommand)] = Define(AuditEntities.Service, AuditActions.Updated),
            [typeof(CreateSupplierCommand)] = Define(AuditEntities.Supplier, AuditActions.Created),
            [typeof(UpdateSupplierCommand)] = Define(AuditEntities.Supplier, AuditActions.Updated),
            [typeof(UpdateUserCommand)] = Define(
                AuditEntities.User,
                AuditActions.Updated,
                request => JsonSerializer.Serialize(new
                {
                    roleId = ((UpdateUserCommand)request).RoleId,
                    isActive = ((UpdateUserCommand)request).IsActive
                }))
        };

    public static bool TryGet(Type commandType, out AuditDefinition definition) =>
        Definitions.TryGetValue(commandType, out definition!);

    private static AuditDefinition Define(
        string entityType,
        string operation,
        Func<object, string?>? newValuesFactory = null) =>
        new(
            AuditActions.For(entityType, operation),
            entityType,
            newValuesFactory ?? (_ => null));
}

internal sealed record AuditDefinition(
    string Action,
    string EntityType,
    Func<object, string?> NewValuesFactory);
