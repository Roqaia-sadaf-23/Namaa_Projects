# Namaa backend implementation status

## Scope and architecture

This document audits the backend against the five-module ownership map in `docs/BUSINESS_MODULES.md`. The implementation preserves the established flow:

`HTTP -> Controller -> Command/Query -> FluentValidation -> Handler -> focused repository -> Result<T> -> HTTP`

Repositories contain persistence logic only. No repository calls `SaveChangesAsync`; mutating handlers call `IApplicationDbContext.SaveChangesAsync`. No generic repository was introduced.

## Entity matrix

| Business module | Feature/entity | Domain | Application | Infrastructure | API | Status and notes |
| --- | --- | --- | --- | --- | --- | --- |
| Project Management | Project | Existing | Existing plus status-change command | Existing | Existing plus `POST /api/projects/{id}/status` | Complete; history is emitted by the aggregate |
| Project Management | ProjectStage | Existing | Existing | Existing | Existing nested routes | Complete; parent-scoped repository access |
| Project Management | ProjectTask | Existing | Existing | Existing | Existing nested routes | Complete; parent-scoped repository access |
| Project Management | ProjectFile | Existing | Existing | Existing | Existing nested routes | Complete metadata API; physical storage remains external |
| Project Management | ProjectMember | Existing | Existing | Existing | Existing nested routes | Complete; remains Project Management-owned |
| Project Management | ProjectCorrespondence | Existing | Create/update/get/list | Focused repository | Project-scoped controller | Complete; schema has no attachment or unique reference constraint |
| Project Management | ProjectStatusHistory | Existing immutable model | Get-by-id/list only | Read-only repository | Project-scoped read routes | Intentionally no public create/update/delete |
| CRM & Sales | Customer | Existing | Existing | Existing | Existing | Complete and preserved |
| CRM & Sales | CustomerContact | Existing child model | Add/update/get/list through Customer aggregate | Customer repository loads contacts | Customer-scoped routes | Complete; parent ID and contact ID are matched together |
| CRM & Sales | Service | Existing | Create/update/get/list | Focused repository | `/api/services` | Complete reference-data surface |
| CRM & Sales | Quotation | Existing aggregate | Create/update/get/list and explicit send/approve/reject/cancel commands | Aggregate repository includes children | `/api/quotations` plus workflow routes | Complete root and supported workflow surface |
| CRM & Sales | QuotationItem | Existing child model | Add through Quotation aggregate | Loaded by Quotation repository | `POST /api/quotations/{id}/items`; returned in root DTO | Aggregate-managed; no independent module/CRUD |
| CRM & Sales | QuotationPaymentTerm | Existing child model | Add through Quotation aggregate | Loaded by Quotation repository | `POST /api/quotations/{id}/payment-terms`; returned in root DTO | Aggregate-managed; ordered by aggregate-generated installment number |
| Finance & Accounting | Account | Existing | Create/update/get/list | Focused repository | `/api/accounts` | Complete; no direct balance mutation exists |
| Finance & Accounting | Invoice | Existing aggregate | Create/update/get/list and explicit issue/cancel commands | Aggregate repository includes items | `/api/invoices` plus workflow routes | Complete supported root workflow |
| Finance & Accounting | InvoiceItem | Existing child model | Add through Invoice aggregate | Loaded by Invoice repository | `POST /api/invoices/{id}/items`; returned in root DTO | Aggregate-managed; totals remain domain-calculated |
| Finance & Accounting | Payment | Existing | Create/update/get/list and explicit post/cancel commands | Focused repository | `/api/payments` plus workflow routes | Complete; no payment-gateway integration invented |
| Finance & Accounting | PaymentVoucher | Existing aggregate | Create/get/list and explicit post/cancel commands | Aggregate repository includes allocations | `/api/payment-vouchers` plus workflow routes | Complete safe surface; posted vouchers cannot be edited |
| Finance & Accounting | PaymentVoucherAllocation | Existing child model | Add through PaymentVoucher aggregate | Loaded by voucher repository | `POST /api/payment-vouchers/{id}/allocations`; returned in root DTO | Aggregate-managed; allocation total cannot exceed voucher amount |
| Finance & Accounting | Expense | Existing | Create/update/get/list | Focused repository | `/api/expenses` | Complete |
| Finance & Accounting | JournalEntry | Existing aggregate | Create/get/list/add-line/post | Aggregate repository includes lines | `/api/journal-entries` | Complete safe surface; posting requires non-empty balanced lines |
| Finance & Accounting | JournalEntryLine | Existing child model | Add through draft JournalEntry | Loaded by journal repository | `POST /api/journal-entries/{id}/lines`; returned in root DTO | Aggregate-managed; no unsafe independent line CRUD |
| Finance & Accounting | CostCenter | Existing | Create/update/get/list | Focused repository | `/api/cost-centers` | Complete hierarchical reference-data surface |
| HR | User | Existing | Get/list/update profile, role and active state; email/password login | Extended focused repository with role-aware auth lookup | `/api/users`; `POST /api/auth/login` | Management and login surfaces complete; password hash never appears in DTOs |
| HR | Role | Existing | Create/update/get/list | Focused repository | `/api/roles` | Complete |
| HR | RefreshToken | Existing support entity | Rotate/revoke through authentication commands | Focused repository stores only the SHA-256 token hash | `POST /api/auth/refresh`; `POST /api/auth/logout` | No generic CRUD; raw token is returned only in auth responses |
| Procurement & Administration | Supplier | Existing | Create/update/get/list | Focused repository | `/api/suppliers` | Complete |
| Procurement & Administration | PurchaseInvoice | Existing aggregate | Create/update/get/list and explicit post/cancel commands | Aggregate repository includes items | `/api/purchase-invoices` plus workflow routes | Complete supported root workflow |
| Procurement & Administration | PurchaseInvoiceItem | Existing child model | Add through PurchaseInvoice aggregate | Loaded by purchase-invoice repository | `POST /api/purchase-invoices/{id}/items`; returned in root DTO | Aggregate-managed |
| Procurement & Administration | Branch | Existing | Create/update/get/list | Focused repository | `/api/branches` | Complete reference-data surface |
| Procurement & Administration | Notification | Existing system-owned model | User-scoped list and mark read/unread | Parent-scoped repository | `/api/users/{userId}/notifications` | Intentionally no public create/delete/general update |
| Procurement & Administration | AuditLog | Existing append-only model | Get-by-id/list with user/entity/date filters | Read-only repository | `/api/audit-logs` | Intentionally no public create/update/delete |

All 32 persisted entities are represented. `DomainGuard` remains an unchanged domain invariant utility and is not a persisted entity.

## Repository audit

Focused abstractions and matching implementations exist for:

- Account, AuditLog, Branch, CostCenter, Customer, Expense, Invoice, JournalEntry, Notification, Payment, PaymentVoucher
- Project, ProjectCorrespondence, ProjectFile, ProjectMember, ProjectStage, ProjectStatusHistory, ProjectTask
- PurchaseInvoice, Quotation, RefreshToken, Role, Service, Supplier, and User

Child entities are intentionally persisted through their aggregate repository. There is no generic repository and no child repository that bypasses aggregate rules.

Every repository is registered in `Namaa.Infrastructure.DependencyInjection`. A source scan confirms zero `SaveChangesAsync` calls under `Infrastructure/Persistence/Repositories`.

## Controller and route audit

Controllers exist for Accounts, Authentication, Audit Logs, Branches, Cost Centers, Customer Contacts, Customers, Expenses, Invoices, Journal Entries, Notifications, Payments, Payment Vouchers, Project Correspondences, Project Files, Project Members, Projects, Project Stages, Project Status History, Project Tasks, Purchase Invoices, Quotations, Roles, Services, Suppliers, Users, and fixed business workflows.

Primary route families:

- `/api/projects`, including nested stages, tasks, files, members, correspondences and status-history
- `/api/customers` and `/api/customers/{customerId}/contacts`
- `/api/services` and `/api/quotations`, including nested items/payment terms and fixed workflow actions
- `/api/accounts`, `/api/invoices`, `/api/payments`, `/api/payment-vouchers`, `/api/expenses`, `/api/journal-entries`, `/api/cost-centers`
- `/api/users`, `/api/roles`, and `/api/users/{userId}/notifications`
- `/api/auth/login`, `/api/auth/refresh`, and `/api/auth/logout`
- `/api/suppliers`, `/api/purchase-invoices`, `/api/branches`, `/api/audit-logs`

Existing routes were not renamed. Collection queries return `Result<IReadOnlyList<TDto>>` and valid empty collections produce `200 OK` with `[]`.

## Validation and result audit

FluentValidation validators exist beside every command and every query with route/filter input. They cover positive IDs, required values, schema lengths, date ordering, numeric ranges, enum values, contact requirements, and debit/credit shape. Parameterless collection queries correctly have no validator.

Expected missing, duplicate, parent mismatch, invalid workflow state, unbalanced journal, and over-allocation outcomes use `Result`/`Result<T>` errors. Controllers continue to use centralized `ResultActionResultExtensions` mapping.

`DomainGuard` remains unchanged. Entity creation and mutation use public aggregate/domain methods; private setters are not bypassed.

## Parent-child protection

Repositories or aggregate loading enforce parent scope for project stages, tasks, files, members, correspondences, status history, customer contacts, notifications, and all newly aggregate-managed child data. Quotation items/terms, invoice items, purchase-invoice items, journal lines, and voucher allocations are loaded and mutated only through their parent aggregate.

## Optimistic concurrency status

The persisted model contains exactly 16 concurrency-enabled entities: `Account`, `Branch`, `CostCenter`, `Customer`, `Expense`, `Invoice`, `JournalEntry`, `Payment`, `PaymentVoucher`, `Project`, `ProjectTask`, `PurchaseInvoice`, `Quotation`, `Service`, `Supplier`, and `User`. Each already had a required SQL Server `timestamp` property configured with both `IsRowVersion()` and `IsConcurrencyToken()`; no token was added to another entity and no schema change was made.

- All 16 root DTOs expose `RowVersion` as a Base64 string. Aggregate-child mutation responses expose the refreshed root token as `ParentRowVersion` for invoice items, journal lines, voucher allocations, purchase-invoice items, quotation items, and quotation payment terms.
- Create requests do not accept a token. Every command that mutates one of the 16 roots implements `IConcurrencyCommand`, and every corresponding API request requires a non-null `RowVersion` string.
- A shared FluentValidation `ConcurrencyCommandValidator<TCommand>` rejects missing, malformed, or empty Base64 tokens. `ConcurrencyBehavior` defensively decodes the validated value and places a copied byte array into the scoped `IConcurrencyTokenContext` for the duration of the command.
- `NamaaDbContext` implements `IConcurrencyTokenContext`. Immediately before saving, it finds modified tracked entities whose mapped `RowVersion` property is an EF concurrency token and assigns the client token to that property's `OriginalValue`. SQL Server therefore remains the authoritative compare-and-update boundary; no extra read or in-memory equality check was introduced.
- `DbUpdateConcurrencyException` is handled centrally by `GlobalExceptionHandler` and returned as `409 Conflict` with code `Concurrency.Conflict` and a non-sensitive reload-and-retry message.

Protected ordinary update requests cover Accounts, Branches, Cost Centers, Customers, Expenses, Invoices, Payments, Projects, Project Tasks, Purchase Invoices, Quotations, Services, Suppliers, and Users. `PaymentVoucher` and `JournalEntry` do not currently have generic update endpoints; all of their existing mutations are protected instead.

Protected aggregate-child operations are invoice-item addition, journal-line addition, payment-voucher allocation, purchase-invoice-item addition, quotation-item addition, and quotation-payment-term addition. Existing parent-scoped repository loading remains unchanged, so a parent mismatch is still `404 Not Found`, while a matching resource with a stale token reaches the database check and becomes `409 Conflict`.

Protected workflow operations are Project status change; Quotation send/approve/reject/cancel; Invoice issue/cancel; Payment post/cancel; PaymentVoucher post/cancel; JournalEntry post; and PurchaseInvoice post/cancel. These workflow responses return the newly generated root token.

No configured disposable database or test project exists, and the task explicitly forbids modifying application data. Consequently, the destructive portion of the suggested live stale-write exercise was not run against the configured application database. Project and Invoice were verified as representative paths by tracing their authenticated API request through an `IConcurrencyCommand`, shared Base64 validation/decoding, tracked aggregate loading, EF `OriginalValue` assignment, awaited `SaveChangesAsync`, refreshed response token, and centralized `DbUpdateConcurrencyException`-to-409 mapping. The safe live acceptance procedure is: read token A, update successfully to token B, retry token A and expect `409`, then read again and confirm the B state remains. This should be executed in an approved disposable environment when test credentials and data are available.

The 16 entities intentionally left without API concurrency tokens are `AuditLog`, `CustomerContact`, `InvoiceItem`, `JournalEntryLine`, `Notification`, `PaymentVoucherAllocation`, `ProjectCorrespondence`, `ProjectFile`, `ProjectMember`, `ProjectStage`, `ProjectStatusHistory`, `PurchaseInvoiceItem`, `QuotationItem`, `QuotationPaymentTerm`, `RefreshToken`, and `Role`.

## Authentication status

- Email is the persisted login identity; there is no username field. Login failure does not distinguish an unknown email from an invalid password.
- Password verification uses ASP.NET Core Identity's versioned PBKDF2 password hasher. Password hashes remain internal and never appear in API DTOs.
- JWT access tokens contain only the user identifier, optional email, role claims, and a token identifier. Configuration keys are `Jwt:Issuer`, `Jwt:Audience`, `Jwt:SigningKey`, `Jwt:AccessTokenMinutes`, and `Jwt:RefreshTokenDays`; the signing key is intentionally absent from tracked settings and is validated at startup.
- Refresh tokens are cryptographically random, SHA-256 hashed into the existing unique `char(64)` column, rotated on refresh, linked through `ReplacedById`, and revoked on logout.
- `ICurrentUser` exposes only `UserId`, `IsAuthenticated`, and role names. The API adapter reads standard identifier and role claims without leaking `HttpContext` into Application.
- Swagger has a Bearer/JWT security definition. The middleware order is authentication, authorization, then endpoint mapping.

## Authorization status

- No authoritative role names exist in source, schema, seed data, or business documentation. No role names were invented.
- Central policy constants are `CanManageProjects`, `CanManageCrmSales`, `CanManageFinance`, `CanPostJournalEntries`, `CanManageUsers`, `CanManageRoles`, `CanManageProcurement`, `CanViewNotifications`, and `CanViewAuditLogs`.
- Until product owners map persisted role names to capabilities, every policy requires an authenticated user only. Role claims are already emitted and ASP.NET Core role authorization remains available for the future mapping.
- Every business controller in the five documented modules is protected through a centralized controller-to-policy convention. Authentication endpoints remain anonymous. Journal posting receives the additional `CanPostJournalEntries` policy, and every explicit workflow action receives its module policy.
- Project Management protection covers Projects, Stages, Tasks, Files, Members, Correspondences, and Status History. CRM & Sales covers Customers, Contacts, Services, and Quotations. Finance covers Accounts, Invoices, Payments, Vouchers, Expenses, Journal Entries, and Cost Centers. HR covers Users and Roles. Procurement & Administration covers Suppliers, Purchase Invoices, Branches, Notifications, and Audit Logs.

## CurrentUser actor migration

The following persisted actor identities now come only from `ICurrentUser.UserId`; their API request fields and command properties were removed:

- Project creator and project-status changer.
- Project-task creator, project-file uploader, and project-correspondence creator.
- Quotation creator and quotation approver.
- Invoice creator and expense creator.
- Payment receiver/recorder and payment poster.
- Payment-voucher payer/creator and voucher poster.
- Journal-entry creator and poster.
- Purchase-invoice creator and poster.

Each affected handler returns the reusable `ApplicationErrors.Unauthenticated` Result error when invoked without a current authenticated user. Project status history, quotation approval, and all posting audit fields now record the JWT user identifier rather than a client assertion.

The following user IDs intentionally remain request-supplied because they identify business targets rather than the caller: `ProjectManagerId`, `AssignedToUserId`, project-member `UserId`, and notification route `userId`. Customer, supplier, account, cost-center, invoice, quotation, and project IDs likewise remain normal business references. Audit Logs still have no public creation endpoint.

This is a deliberate request-contract migration: the removed actor fields are no longer accepted by create/status/post/approve request bodies. Routes, response DTOs, persisted fields, and Domain method signatures remain unchanged.

No test project exists, so this hardening phase did not introduce a new testing framework. Future authorization tests should cover anonymous rejection, authenticated access, JWT-derived actor persistence, actor-field spoofing rejection, configured role-policy denial, workflow audit actors, and notification ownership.

## Security and infrastructure gaps

- User onboarding, password change/reset, lockout, MFA, and security-stamp invalidation remain undefined and are not exposed.
- Existing password hashes have no documented algorithm marker outside the hash itself. Login now accepts ASP.NET Core Identity password-hash formats; any legacy format requires an explicit compatibility/migration decision. Users whose persisted Email is null cannot use the email login endpoint.
- Persisted role-to-policy mappings and segregation-of-duties rules remain unresolved for finance, HR, audit logs, project membership/files, and accounting posting. Policies therefore enforce authentication but not a fabricated role mapping.
- The user-scoped Notifications route is authenticated but still needs an ownership-versus-administrator authorization decision.
- Project-level membership/ACL authorization is not yet defined; authenticated users currently share the same module policy boundary.
- Quotation rejection/cancellation, invoice issue/cancellation, and cancellation workflows do not have actor columns in the current Domain/schema, so no audit actor can be persisted for those transitions without a later schema/business decision.
- Workflow timestamps such as `PostedAt` remain caller-supplied to preserve current contracts; only actor identity was hardened in this phase.
- Project Files still store metadata and `StorageKey` only. No physical storage provider was added.
- A live two-client concurrency acceptance run remains pending for an approved disposable database because application-data mutation was out of scope for this hardening task.

## Remaining business decisions

- Define user onboarding, password reset/change, lockout/MFA, and security-stamp/token-family invalidation.
- Define named authorization policies and segregation-of-duties rules for posting, HR management, notification ownership, and audit access.
- Decide whether child-line replacement/removal is required before documents leave Draft; the current API supports safe addition and aggregate reads without exposing unsafe independent CRUD.
- Decide whether notification creation is handled by domain events, an outbox, or an application notification service.
- Define automatic audit-log capture and redaction policy.
- Select a physical file-storage abstraction and provider.
- Decide whether a future HTTP `ETag`/`If-Match` facade should supplement the current JSON Base64 `RowVersion` contract; the present contract is complete and consistent.

## Database and build safety

- No migration was created or run.
- No database update was executed.
- No schema, connection string, credentials, seed data, or application data was modified.
- Final build command: `dotnet build Namaa.sln -m:1` using isolated artifact output through `UseArtifactsOutput`/`ArtifactsPath` because the normal output can be held by an external process.
- Final result: **0 warnings, 0 errors**.
