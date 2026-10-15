# Namaa business module map

## Decision

Namaa exposes exactly five high-level business modules to product navigation, planning, and future dashboard/sidebar surfaces:

1. Project Management
2. CRM & Sales
3. Finance & Accounting
4. HR
5. Procurement & Administration

This is a business ownership boundary, not an instruction to merge technical features or persisted models. Each command, query, validator, handler, DTO, repository, entity, EF configuration, controller, contract, route, table, and relationship keeps its current responsibility.

## Architectural boundary

The module map preserves the established request flow:

`HTTP request -> Controller -> Command/Query -> FluentValidation -> Handler -> Repository -> Result<T> -> Controller -> HTTP response`

- Repositories perform persistence operations only and do not call `SaveChangesAsync`.
- Handlers coordinate use cases and call `IApplicationDbContext.SaveChangesAsync`.
- MediatR, FluentValidation, the Result Pattern, centralized Result-to-HTTP mapping, focused repositories, one responsibility per file, and `DomainGuard` invariants remain unchanged.
- A business module may contain several independently implemented technical features. Sharing a parent module does not combine their application or persistence code.

## Complete entity and feature inventory

The current schema, Domain model, EF configurations, application folders, repositories, and controllers were inspected. There are 32 persisted entities. `DomainGuard` is also present, but it is a domain invariant utility rather than a persisted entity or business feature.

| Business module | Feature | Persisted entities | Role in the module | Current delivery state |
| --- | --- | --- | --- | --- |
| Project Management | Projects | `Project` | Primary feature | Application use cases, repository, controller, and routes implemented |
| Project Management | Project Stages | `ProjectStage` | Separate technical feature under Project Management | Application use cases, repository, controller, and routes implemented |
| Project Management | Project Tasks | `ProjectTask` | Separate technical feature under Project Management | Application use cases, repository, controller, and routes implemented |
| Project Management | Project Files | `ProjectFile` | Separate technical feature under Project Management | Application use cases, repository, controller, and routes implemented |
| Project Management | Project Members | `ProjectMember` | Separate operational project-membership feature; not an HR feature | Application use cases, repository, controller, and routes implemented |
| Project Management | Project Correspondences | `ProjectCorrespondence` | Separate technical feature under Project Management | Domain entity and EF configuration present; application/API feature pending |
| Project Management | Project Status History | `ProjectStatusHistory` | Project lifecycle/history subfeature | Domain entity and EF configuration present; application/API feature pending |
| CRM & Sales | Customers | `Customer` | Primary feature | Application use cases, repository, controller, and routes implemented |
| CRM & Sales | Customer Contacts | `CustomerContact` | Child/subfeature of Customers | Domain entity and EF configuration present; managed through the Customer aggregate model |
| CRM & Sales | Services | `Service` | Sales service catalog feature | Domain entity and EF configuration present; application/API feature pending |
| CRM & Sales | Quotations | `Quotation` | Primary feature | Domain entity, EF configuration, and repository support present; application/API use cases pending |
| CRM & Sales | Quotation Items | `QuotationItem` | Child/subfeature of Quotations | Domain entity and EF configuration present |
| CRM & Sales | Quotation Payment Terms | `QuotationPaymentTerm` | Child/subfeature of Quotations | Domain entity and EF configuration present |
| Finance & Accounting | Accounts | `Account` | Chart-of-accounts feature | Domain entity and EF configuration present; application/API feature pending |
| Finance & Accounting | Invoices | `Invoice` | Primary finance feature | Domain entity and EF configuration present; application/API feature pending |
| Finance & Accounting | Invoice Items | `InvoiceItem` | Child/subfeature of Invoices | Domain entity and EF configuration present |
| Finance & Accounting | Payments | `Payment` | Independent finance feature | Domain entity and EF configuration present; application/API feature pending |
| Finance & Accounting | Payment Vouchers | `PaymentVoucher` | Independent finance feature | Domain entity and EF configuration present; application/API feature pending |
| Finance & Accounting | Payment Voucher Allocations | `PaymentVoucherAllocation` | Child/subfeature of Payment Vouchers | Domain entity and EF configuration present |
| Finance & Accounting | Expenses | `Expense` | Independent finance feature | Domain entity and EF configuration present; application/API feature pending |
| Finance & Accounting | Journal Entries | `JournalEntry` | Independent finance feature | Domain entity and EF configuration present; application/API feature pending |
| Finance & Accounting | Journal Entry Lines | `JournalEntryLine` | Child/subfeature of Journal Entries | Domain entity and EF configuration present |
| Finance & Accounting | Cost Centers | `CostCenter` | Independent finance feature | Domain entity and EF configuration present; application/API feature pending |
| HR | Users / Employee Management | `User` | Employee/user identity feature | Domain entity, EF configuration, and repository support present; application/API use cases pending |
| HR | Roles | `Role` | User role-management feature | Domain entity and EF configuration present; application/API feature pending |
| HR | Authentication Support | `RefreshToken` | Child/supporting identity entity; not a separate top-level business feature | Domain entity and EF configuration present; authentication is not implemented by this organization change |
| Procurement & Administration | Suppliers | `Supplier` | Primary procurement feature | Domain entity and EF configuration present; application/API feature pending |
| Procurement & Administration | Purchase Invoices | `PurchaseInvoice` | Primary procurement feature | Domain entity and EF configuration present; application/API feature pending |
| Procurement & Administration | Purchase Invoice Items | `PurchaseInvoiceItem` | Child/subfeature of Purchase Invoices | Domain entity and EF configuration present |
| Procurement & Administration | Branches | `Branch` | Administrative organization/configuration feature | Domain entity and EF configuration present; application/API feature pending |
| Procurement & Administration | Notifications | `Notification` | Cross-cutting capability owned by Administration | Domain entity and EF configuration present; application/API feature pending |
| Procurement & Administration | Audit Logs | `AuditLog` | Cross-cutting governance capability owned by Administration | Domain entity and EF configuration present; application/API feature pending |

All 32 persisted entities are assigned once in this ownership map. Relationships may cross modules without changing ownership or database relationships.

## Final module hierarchy

### Project Management

- Projects
- Project Stages
- Project Tasks
- Project Files
- Project Members
- Project Correspondences
- Project Status History

### CRM & Sales

- Customers
  - Customer Contacts
- Services
- Quotations
  - Quotation Items
  - Quotation Payment Terms

### Finance & Accounting

- Accounts
- Invoices
  - Invoice Items
- Payments
- Payment Vouchers
  - Payment Voucher Allocations
- Expenses
- Journal Entries
  - Journal Entry Lines
- Cost Centers

### HR

- Users / Employee Management
- Roles
- Authentication Support
  - Refresh Tokens

Project Members are intentionally excluded from HR ownership. HR owns user identity and roles; Project Management owns the relationship that assigns a user to a project.

### Procurement & Administration

- Suppliers
- Purchase Invoices
  - Purchase Invoice Items
- Branches
- Notifications
- Audit Logs

## Dashboard and API interpretation

The future dashboard/sidebar should show only the five module names as top-level navigation. Features listed beneath a module are child navigation items, not additional top-level modules.

This navigation grouping does not introduce a new API route prefix. Existing controllers and routes remain unchanged:

- `api/customers`
- `api/projects`
- `api/projects/{projectId}/stages`
- `api/projects/{projectId}/tasks`
- `api/projects/{projectId}/files`
- `api/projects/{projectId}/members`

New endpoints should follow the established resource conventions and should not force existing endpoints beneath business-module URL segments.

## Physical organization decision

No existing runtime folders or namespaces were moved. The completed application features currently use flat folders such as `Customers`, `Projects`, `ProjectStages`, `ProjectTasks`, `ProjectFiles`, and `ProjectMembers`; controllers, contracts, repository abstractions, and repository implementations use corresponding established locations.

Moving those files would create widespread namespace and reference churn without changing business behavior. It would also add regression risk to completed features and in-progress working-tree changes. The documented map is therefore the authoritative grouping. Physical moves may be made incrementally only when a feature is already being changed and route/API compatibility can be verified.

For future application features, the conceptual destination is:

```text
Namaa.Application
|-- ProjectManagement
|   |-- Projects
|   |-- ProjectStages
|   |-- ProjectTasks
|   |-- ProjectFiles
|   |-- ProjectMembers
|   |-- ProjectCorrespondences
|   `-- ProjectStatusHistory
|-- CrmSales
|   |-- Customers
|   |-- Services
|   `-- Quotations
|-- FinanceAccounting
|   |-- Accounts
|   |-- Invoices
|   |-- Payments
|   |-- PaymentVouchers
|   |-- Expenses
|   |-- JournalEntries
|   `-- CostCenters
|-- HR
|   |-- Users
|   `-- Roles
`-- ProcurementAdministration
    |-- Suppliers
    |-- PurchaseInvoices
    |-- Branches
    |-- Notifications
    `-- AuditLogs
```

Child entities remain inside their parent feature unless an existing or future independent use case justifies a separate technical feature. This convention never implies table or entity merging.

## Placement decisions and remaining business questions

The following entities were present but not explicit in the initial examples and are now assigned:

- `CustomerContact` belongs to CRM & Sales as a child of Customers.
- `Account` belongs to Finance & Accounting as the chart-of-accounts feature.
- `RefreshToken` belongs to HR as authentication-support data beneath Users; this does not authorize authentication implementation.

Cross-cutting or relationship-based placements are resolved as follows:

- `ProjectMember` remains in Project Management even though it references `User`.
- `Branch` is owned by Procurement & Administration even though Finance entities reference branches.
- `Notification` and `AuditLog` are system-wide capabilities with administrative ownership.
- `CostCenter` remains in Finance & Accounting even though it can reference a branch.
- `Service` remains in CRM & Sales even though invoices can reference services.

No entity placement is currently unresolved. Product owners may later decide whether Accounts, Branches, Notifications, Audit Logs, or authentication support should be visible sidebar entries or remain configuration/support screens; that presentation choice does not change module ownership.

## Safety record for this organization change

- No entity was merged or renamed.
- No table was merged, renamed, added, or removed.
- No relationship or EF configuration was changed.
- No migration was created or run.
- No database update was executed.
- No database schema or application data was modified.
- No connection string was changed or exposed.
- Existing commands, queries, validators, handlers, repositories, DTOs, controllers, contracts, routes, and Result behavior were preserved.
