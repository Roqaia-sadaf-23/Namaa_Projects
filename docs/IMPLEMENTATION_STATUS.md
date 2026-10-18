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
| Project Management | ProjectFile | Existing | Existing | Existing | Protected metadata, upload, and download routes | Complete local physical-storage flow; cloud provider deferred |
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
| Procurement & Administration | Notification | Existing system-owned model | Event-driven creation plus self-scoped get/list and mark read/unread | Recipient-scoped repository plus transactional writer | `/api/notifications` | No public create/delete/general update; ownership comes from JWT current user |
| Procurement & Administration | AuditLog | Existing append-only model | Central automated write behavior plus get-by-id/list filters | Transactional `EfAuditTrail` writer plus read-only repository | Read-only `/api/audit-logs` | No public create/update/delete; trusted application writes only |

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
- `/api/users`, `/api/roles`, and self-service `/api/notifications`
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
- SQL Server `rowversion` is exactly 8 bytes and the API represents it as Base64. A shared FluentValidation `ConcurrencyCommandValidator<TCommand>` rejects null, empty, whitespace, malformed Base64, and any Base64 value that does not decode to exactly 8 bytes. These invalid inputs produce `400 Bad Request`; `ConcurrencyBehavior` repeats the exact-length check defensively and places a copied byte array into the scoped `IConcurrencyTokenContext` for the duration of the command.
- `NamaaDbContext` implements `IConcurrencyTokenContext`. Immediately before saving, it finds modified tracked entities whose mapped `RowVersion` property is an EF concurrency token and assigns the client token to that property's `OriginalValue`. The ambient token supports exactly one modified RowVersion-enabled root per command. If more than one is found, an `InvalidOperationException` reports the unsupported command design and reaches the existing internal-error path rather than being misreported as a client concurrency conflict. SQL Server remains the authoritative compare-and-update boundary; no extra read or in-memory equality check was introduced.
- `DbUpdateConcurrencyException` is handled centrally by `GlobalExceptionHandler` and returned as `409 Conflict` with code `Concurrency.Conflict` and a non-sensitive reload-and-retry message.
- The focused `tests/Namaa.Tests` project covers exact 8-byte token decoding, malformed-token rejection before handler execution, expected-token propagation and cleanup in `ConcurrencyBehavior`, and rejection of multiple modified RowVersion roots for one ambient token. The final run passed all 13 test cases with 0 failures.

Protected ordinary update requests cover Accounts, Branches, Cost Centers, Customers, Expenses, Invoices, Payments, Projects, Project Tasks, Purchase Invoices, Quotations, Services, Suppliers, and Users. `PaymentVoucher` and `JournalEntry` do not currently have generic update endpoints; all of their existing mutations are protected instead.

Protected aggregate-child operations are invoice-item addition, journal-line addition, payment-voucher allocation, purchase-invoice-item addition, quotation-item addition, and quotation-payment-term addition. Each command implements `IConcurrencyCommand`, loads the tracked parent root for update, mutates that parent, awaits `SaveChangesAsync`, and only then maps the child DTO from its parent navigation. Its `ParentRowVersion` therefore comes from the refreshed parent token. Existing parent-scoped repository loading remains unchanged, so a parent mismatch is still `404 Not Found`, while a matching resource with a stale token reaches the database check and becomes `409 Conflict`.

Protected workflow operations are Project status change; Quotation send/approve/reject/cancel; Invoice issue/cancel; Payment post/cancel; PaymentVoucher post/cancel; JournalEntry post; and PurchaseInvoice post/cancel. These workflow responses return the newly generated root token.

The API acceptance suite now proves the concurrency contract against SQL Server LocalDB. It GETs Project and Account tokens, performs an accepted update, verifies the native SQL Server token changes, retries with the stale token, observes `409 Conflict`, and confirms both the accepted state and the single success audit remain unchanged. Malformed Base64 plus decoded lengths 1, 7, and 9 are rejected with `400` before persistence; only the 8-byte transport format reaches the handler.

All mutation handlers materialize their response DTO after awaited `SaveChangesAsync`, so successful root responses read the store-generated new `RowVersion`; the six aggregate-child responses likewise read the refreshed parent token as `ParentRowVersion`.

The 16 entities intentionally left without API concurrency tokens are `AuditLog`, `CustomerContact`, `InvoiceItem`, `JournalEntryLine`, `Notification`, `PaymentVoucherAllocation`, `ProjectCorrespondence`, `ProjectFile`, `ProjectMember`, `ProjectStage`, `ProjectStatusHistory`, `PurchaseInvoiceItem`, `QuotationItem`, `QuotationPaymentTerm`, `RefreshToken`, and `Role`.

## Authentication status

- Email is the persisted login identity; there is no username field. Login failure does not distinguish an unknown email from an invalid password.
- Password verification uses ASP.NET Core Identity's versioned PBKDF2 password hasher. Password hashes remain internal and never appear in API DTOs.
- JWT access tokens contain only the user identifier, optional email, role claims, and a token identifier. Configuration keys are `Jwt:Issuer`, `Jwt:Audience`, `Jwt:SigningKey`, `Jwt:AccessTokenMinutes`, and `Jwt:RefreshTokenDays`; the signing key is intentionally absent from tracked settings and is validated at startup.
- Refresh tokens are cryptographically random, SHA-256 hashed into the existing unique `char(64)` column, rotated on refresh, linked through `ReplacedById`, and revoked on logout.
- `ICurrentUser` exposes only `UserId`, `IsAuthenticated`, and role names. The API adapter reads standard identifier and role claims without leaking `HttpContext` into Application.
- Swagger has a Bearer/JWT security definition. The middleware order is authentication, authorization, then endpoint mapping.

## Roles and Authorization

No authoritative role catalog existed in source, schema, seed data, README, business documentation, or frontend assets before this hardening phase. The initial catalog is therefore explicitly defined in one Application source, `RoleNames`: `Administrator`, `ProjectManager`, `Sales`, `Accountant`, `HR`, `Procurement`, and `Employee`. Role values remain data rows; source constants do not provision or mutate a database.

Capability policies remain the stable API boundary. The centralized policy-to-role map is:

| Policy | Allowed roles |
| --- | --- |
| `CanManageProjects` | `Administrator`, `ProjectManager` |
| `CanManageCrmSales` | `Administrator`, `Sales` |
| `CanManageFinance` | `Administrator`, `Accountant` |
| `CanPostJournalEntries` | `Administrator`, `Accountant` |
| `CanManageUsers` | `Administrator`, `HR` |
| `CanManageRoles` | `Administrator` |
| `CanManageProcurement` | `Administrator`, `Procurement` |
| `CanViewNotifications` | Any authenticated user; repository ownership remains mandatory |
| `CanViewAuditLogs` | `Administrator` |

The source currently contains 27 controllers, including the intentionally anonymous authentication controller and the action-policy-only workflow controller. Their coverage is:

| Controller | Module | Policy | Allowed roles |
| --- | --- | --- | --- |
| Accounts | Finance & Accounting | `CanManageFinance` | Administrator, Accountant |
| AuditLogs | Administration | `CanViewAuditLogs` | Administrator |
| Auth | Authentication | `AllowAnonymous` | Anonymous/authenticated callers as appropriate to login, refresh, logout |
| Branches | Procurement & Administration | `CanManageProcurement` | Administrator, Procurement |
| BusinessWorkflows | Cross-module workflows | Action-specific CRM, Finance, or Procurement policy | Administrator plus Sales, Accountant, or Procurement according to action |
| CostCenters | Finance & Accounting | `CanManageFinance` | Administrator, Accountant |
| CustomerContacts | CRM & Sales | `CanManageCrmSales` | Administrator, Sales |
| Customers | CRM & Sales | `CanManageCrmSales` | Administrator, Sales |
| Expenses | Finance & Accounting | `CanManageFinance` | Administrator, Accountant |
| Invoices | Finance & Accounting | `CanManageFinance` | Administrator, Accountant |
| JournalEntries | Finance & Accounting | `CanManageFinance`; Post also `CanPostJournalEntries` | Administrator, Accountant |
| Notifications | Administration/self-service | `CanViewNotifications` plus recipient ownership | Any authenticated user, own rows only |
| Payments | Finance & Accounting | `CanManageFinance` | Administrator, Accountant |
| PaymentVouchers | Finance & Accounting | `CanManageFinance` | Administrator, Accountant |
| ProjectCorrespondences | Project Management | `CanManageProjects` | Administrator, ProjectManager |
| ProjectFiles | Project Management | `CanManageProjects` | Administrator, ProjectManager |
| ProjectMembers | Project Management | `CanManageProjects` | Administrator, ProjectManager |
| Projects | Project Management | `CanManageProjects` | Administrator, ProjectManager |
| ProjectStages | Project Management | `CanManageProjects` | Administrator, ProjectManager |
| ProjectStatusHistory | Project Management | `CanManageProjects` | Administrator, ProjectManager |
| ProjectTasks | Project Management | `CanManageProjects` | Administrator, ProjectManager |
| PurchaseInvoices | Procurement & Administration | `CanManageProcurement` | Administrator, Procurement |
| Quotations | CRM & Sales | `CanManageCrmSales` | Administrator, Sales |
| Roles | HR/identity administration | `CanManageRoles` | Administrator |
| Services | CRM & Sales | `CanManageCrmSales` | Administrator, Sales |
| Suppliers | Procurement & Administration | `CanManageProcurement` | Administrator, Procurement |
| Users | HR | `CanManageUsers` | Administrator, HR |

Project status changes retain `CanManageProjects`; quotation transitions retain `CanManageCrmSales`; invoice issue/cancel, payment post/cancel, and voucher post/cancel retain `CanManageFinance`; journal posting requires both its controller finance policy and `CanPostJournalEntries`; purchase-invoice post/cancel retains `CanManageProcurement`. No maker-checker rule was invented because the current model defines no such restriction.

Unauthenticated protected requests produce `401`; authenticated principals without an allowed role produce `403`. Authorization failures occur before MediatR and create no success business audit. Swagger Bearer support remains unchanged; a displayed lock indicates authentication while the role claim determines whether a request is authorized.

JWT login and refresh load `User.Role.Name` and emit that exact canonical value as a standard role claim. A role change does not revoke already issued access tokens, so the old capability remains until the short-lived token expires; security-stamp/token-version invalidation is future work. Module roles do not implement record-level or project-membership ACLs. Notification access remains independently owner-scoped even for `Administrator`.

No production role rows are inserted automatically. Each deployment must provision the seven exact role names before assigning users. The acceptance fixture alone creates those roles in its random disposable database. Production seeding, maker-checker segregation of duties, project-level ACLs, and any future audit-specific role require separate approved decisions.

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

The following user IDs intentionally remain request-supplied because they identify business targets rather than the caller: `ProjectManagerId`, `AssignedToUserId`, and project-member `UserId`. Notification ownership no longer accepts a route user ID and comes exclusively from `ICurrentUser`. Customer, supplier, account, cost-center, invoice, quotation, and project IDs likewise remain normal business references. Audit Logs still have no public creation endpoint.

This is a deliberate request-contract migration: the removed actor fields are no longer accepted by create/status/post/approve request bodies. Routes, response DTOs, persisted fields, and Domain method signatures remain unchanged.

SQL-backed acceptance tests cover anonymous rejection, role-specific allow/deny behavior, JWT-derived actor persistence, actor-field spoofing rejection, workflow audit actors, and notification ownership.

## Technical Logging

- `Namaa.Api` uses Serilog.AspNetCore 10.0.0 with the compatible `net8.0` asset group, Serilog.Sinks.Console 6.1.1, and Serilog.Sinks.File 7.0.0. Serilog is the provider behind `ILogger<T>`; direct static `Log` calls are limited to bootstrap startup, fatal host failure, and shutdown.
- Configuration is centralized in `TechnicalLogging` and `appsettings*.json`. Structured events are enriched from `LogContext` and with `ApplicationName` and `EnvironmentName`. Defaults are `Information`, with framework and EF database-command noise reduced to `Warning`; the Development override enables `Debug` only for `Namaa` sources.
- Sinks are console plus `logs/nam-aa-.log`, rolled daily and retained for 14 files. The existing `.gitignore` already excludes `Logs/`, `logs/`, and `*.log`.
- `UseSerilogRequestLogging` records one completion event containing request method, path, status, and elapsed time. It is the outer request boundary, followed by the existing exception handler, HTTPS redirection, authentication, and authorization. Request bodies, query contents, headers, authorization headers, and token contents are not captured.
- `GlobalExceptionHandler` remains the authoritative request-exception logger. It records expected request failures as structured warnings and unexpected exceptions as errors; Serilog request logging records the resulting status/duration event without a second stack trace. Bootstrap failures are logged once by the host boundary.
- No log call or enricher reads plaintext passwords, `PasswordHash`, access/refresh tokens, JWT contents or signing keys, authorization headers, connection strings, database passwords, request bodies, or secret configuration values. EF SQL command logging remains at `Warning`, and sensitive-data logging is not enabled.

## Business Audit Logging

The existing `AuditLog` table/model is reused unchanged. Its actual fields are `Id` (`long`), nullable `UserId`, required `Action`, required `EntityType`, nullable `EntityId`, nullable JSON `OldValues`, nullable JSON `NewValues`, nullable `IpAddress`, and `CreatedAt`. Records expose no public setters or mutation methods. No create, update, or delete AuditLog API exists; the existing read-only get/list endpoints remain protected by `CanViewAuditLogs`.

- `AuditBehavior<TRequest,TResponse>` is the single application-level mechanism. A central command catalog defines stable `<Entity>.<Action>` names and covers all 62 current non-authentication mutation commands. Reads and the login, refresh, and logout commands are intentionally excluded.
- `IAuditTrail` keeps EF Core out of Application. Its `EfAuditTrail` implementation opens one database transaction, runs the existing handler and its business `SaveChangesAsync`, creates exactly one trusted `AuditLog` only for a successful `Result<T>`, saves that row, and commits. A failed Result is rolled back; validation/authorization failures never enter the behavior; exceptions, cancellation, audit persistence failure, and stale `DbUpdateConcurrencyException` dispose the uncommitted transaction so neither the business write nor a success audit is committed.
- Actor identity comes exclusively from `ICurrentUser.UserId`. Request user IDs remain business targets only and cannot select the audit actor. The nullable `UserId` schema can represent a deliberately unauthenticated internal invocation without inventing a user ID, although all current business HTTP controllers require authentication.
- Covered Project Management actions include create/update/status changes plus task, file metadata, member, stage, and correspondence mutations. CRM/Sales covers customer/contact/service/quotation mutations, line/term additions, and quotation workflow transitions. Finance covers accounts, invoices and lines, payments, vouchers and allocations, expenses, journal entries/lines/posting, and cost centers. HR covers role create/update and user profile/role/active-state updates. Procurement covers suppliers, purchase invoices/lines/workflows, and branches. Notification read-state changes are also captured.
- Safe metadata is intentionally narrow: project status changes store only the destination status, and user updates store only the requested role ID and active flag in `NewValues`. Generic request bodies, full entity snapshots, credentials, tokens, contact details, free-text notes, and authentication data are not serialized. `OldValues` and `IpAddress` remain null because no trustworthy generic before-state or client-address requirement was introduced.
- Optimistic concurrency remains authoritative. A successful mutation and its audit share the explicit transaction; a stale write produces the existing `409 Conflict` and no success audit record. Audit insertion failures propagate to the existing exception boundary and are not swallowed.
- Focused tests cover successful/failed mutations, validation short-circuiting, stale-write exceptions, actor spoof resistance, action/entity naming, full catalog coverage, authentication-command exclusion, immutable AuditLog shape, and Serilog initialization without secret configuration enrichment.

Current gaps are intentionally limited: there is no public user-create command to audit as `User.Created`, and full before/after capture and IP capture are deferred pending explicit privacy and trust-boundary requirements. Live SQL Server acceptance now confirms successful business writes and audits commit together while validation failures and stale writes create neither a business overwrite nor a success audit. A deliberately injected audit-sink failure test remains optional because no production-only test seam was added.

## Integration / Acceptance Testing

- Test host: `WebApplicationFactory<Program>` runs the production controller, MediatR, FluentValidation, authentication, authorization, repository, EF Core, exception, audit, concurrency, and Serilog request pipelines. `Program` is exposed only through the minimal public partial entry-point declaration. In `Testing`, startup exceptions are rethrown after fatal logging so host failures are visible to the test runner; production startup behavior is unchanged.
- Database strategy: the collection fixture creates a random `NamaaAcceptance_<guid>` database, applies the tracked authoritative `Database/NamaaDatabaseSchema.sql` in `GO` batches while deliberately ignoring its fixed `USE [NamaaProjects]` batch, and drops the database after the collection. The default developer path is isolated `(localdb)\MSSQLLocalDB`. CI can set `NAMAA_TEST_SQLSERVER_CONNECTION_STRING` to a dedicated SQL Server instance credential with create/drop-database permission; the fixture discards any catalog in that value, connects to `master`, and still creates its own random database. It never reads `DefaultConnection` or user secrets.
- Test data and credentials: a random signing key, random passwords, unique test emails/codes, the seven canonical test roles, active and inactive users, a customer, and scenario-specific records are created only inside the disposable database. Host environment values are restored at fixture teardown. No password, key, token, connection string, or database credential is committed.
- Isolation: SQL-backed acceptance classes share one disposable database collection and are explicitly non-parallel. Every scenario creates uniquely identified business records and scopes persistence/audit assertions to those records, so tests have no execution-order dependency.
- Authentication coverage: valid login, generic wrong-password/unknown-user parity, inactive-user rejection, password-hash response exclusion, SHA-256-only refresh-token persistence, refresh rotation with old-token rejection and new-token acceptance, and logout revocation.
- Authorization coverage: anonymous protected access returns `401`; authenticated wrong-role access returns `403`. Administrator reaches every capability, ProjectManager is project-only, Sales is CRM-only, Accountant manages finance and posts journals, HR manages users but not roles, Procurement is procurement-only, Employee is denied module management, and AuditLog reads are Administrator-only. Notification ownership still returns `404` for another recipient's row even to Administrator.
- Project/audit coverage: JWT-derived creator identity wins even when an unknown spoofed actor property is supplied, the business row and `Project.Created` audit share the actor, invalid creation changes neither table, and Project stale updates return `409` without overwriting or adding a second success audit.
- Finance/workflow coverage: Account stale updates use live rowversion behavior; balanced journals post and write `JournalEntry.Posted`; unbalanced journals remain Draft and have no post audit; voucher over-allocation returns `409` with neither allocation nor success audit.
- Security coverage: a task from project B requested through project A returns `404`; project-file metadata requires authentication and its intentionally hidden `StorageKey` is absent; AuditLog has authenticated GET routes only and POST/PUT/DELETE return `405`; the in-memory Serilog sink sees request events without passwords, bearer/access/refresh tokens, signing keys, authorization values, or connection strings.
- Count/result: 41 SQL-backed API acceptance cases plus 38 focused/unit cases, for 79 total passing tests. The SQL acceptance cases exercise native SQL Server `timestamp/rowversion`, foreign keys, transactions, notification persistence, role-specific authorization, and temporary local file storage rather than EF InMemory or SQLite.
- Command line: `dotnet build Namaa.sln -m:1`, then `dotnet test Namaa.sln -m:1`. Developers need SQL Server LocalDB; CI should provide `NAMAA_TEST_SQLSERVER_CONNECTION_STRING` without checking credentials into source control.
- Remaining acceptance gaps: an injected audit-insert failure test was not added because doing so would require a production test seam; container execution is not needed while LocalDB/dedicated SQL Server is available.

## Physical File Storage

- Architecture: Application defines provider-neutral `IFileStorage`, `IFileUploadPolicy`, safe stored-file metadata, upload orchestration, and download lookup. Infrastructure implements the first provider as `LocalFileStorage`; Domain retains only `ProjectFile` invariants; API owns multipart binding and streamed HTTP responses. The abstraction can later be implemented by Azure Blob Storage, Amazon S3, or another object store without moving filesystem concerns into Application or Domain.
- Configuration: tracked settings select `FileStorage:Provider=Local`, a 25 MiB (`26214400` byte) maximum, and configurable blocked extensions/content types. `LocalRoot` is intentionally absent from tracked settings. When omitted, the Local provider uses the current account's OS-local application-data directory under `Namaa/ProjectFiles`; deployments can set an absolute `FileStorage__LocalRoot`. Relative configured roots are rejected at startup.
- Upload: authenticated clients use `POST /api/projects/{projectId}/files/upload` with `multipart/form-data` fields `file` and optional `description`. The former public metadata POST that accepted `StorageKey`, size, uploader, and timestamp assertions was removed; the existing metadata command remains internal for compatibility with trusted application callers only. `UploadedByUserId` comes from `ICurrentUser`.
- Keys and path safety: the provider generates collision-resistant keys shaped like `objects/<prefix>/<guid>` and never incorporates the original filename, email, or physical root. Resolution accepts only that strict opaque format, canonicalizes the path, and enforces the configured root. Benign client path components are stripped to a safe basename; traversal segments, absolute paths, Windows drive/UNC paths, encoded traversal variants, and control characters are rejected. Rejected storage-key validation is logged without the key or physical path. The sanitized metadata filename is used through the framework file-response helper for attachment headers. `StorageKey` and physical paths remain absent from read/upload DTOs and HTTP responses.
- Validation policy: empty files, declared files over the configured maximum, invalid filename/content-type metadata, and configured blocked executable/script extensions or risky content types return the existing `400 Bad Request` validation result. The Local provider independently counts bytes while copying and stops if the real stream exceeds the maximum, so a false multipart length cannot bypass the limit. Extension and content type are advisory policy signals; no claim is made that client `Content-Type` identifies file contents.
- Streaming: uploads are read from `IFormFile.OpenReadStream` and copied incrementally with a pooled buffer into a staging file; no production `byte[]`, whole-file `MemoryStream`, Base64 payload, or SQL binary column is used. A completed staging file is atomically moved to its opaque destination. Downloads use `GET /api/projects/{projectId}/files/{id}/download` and return a `FileStreamResult` with the sanitized original name, safe attachment disposition, and stored content type (or `application/octet-stream`).
- Authorization and scoping: upload, metadata reads/updates, and download retain `CanManageProjects`, allowing Administrator and ProjectManager. The download query performs the existing project-and-file scoped database lookup before opening storage, so a file requested through another project returns `404` without disclosing its existence.
- Drift and logging: metadata with missing physical content returns a path-free `404` and emits a structured warning containing only project/file identifiers. Storage, download, compensation, and staging-cleanup failures emit structured failure type and safe business identifiers; file bytes, tokens, authorization headers, keys, physical paths, and secret configuration are not logged.
- Consistency: after storage succeeds, the upload handler persists metadata inside the existing audit transaction. A metadata `SaveChangesAsync` failure triggers best-effort physical deletion and rethrows; cleanup failure is logged without a path. Storage or validation failure returns before metadata/audit creation. Successful upload writes `ProjectFile.MetadataCreated` with the JWT actor and no path, key, or file content. Because SQL and a filesystem cannot share one atomic transaction, a process crash or a later outer audit-commit failure can still leave an orphan; reconciliation/retention tooling is a future operational concern.
- Deletion: no public DELETE was added because no approved ProjectFile deletion/archive business rule exists. The provider delete operation is currently used only for upload compensation and tests.
- Tests: eight SQL-backed acceptance cases cover authenticated/anonymous upload, JWT actor, spoofed-key rejection, physical existence, hidden key, streamed download, cross-project `404`, missing content, maximum size, traversal rejection, blocked types, audit success/failure behavior, and temporary-root cleanup. Five focused policy tests cover empty names, traversal variants, declared-size limits, normalized valid metadata, and configured blocked extensions. Six focused Local-provider tests cover independent actual-byte enforcement, successful physical persistence, existence, streamed reads, deletion, and traversal/absolute storage-key rejection. A focused handler test forces metadata persistence failure and proves compensation deletes the stored object. Every storage test uses a unique OS temporary root and removes it during teardown.

## Notification Engine

- Architecture: an Application pipeline behavior maps successful command results to typed MediatR notification events. Event handlers use `INotificationWriter`; the Infrastructure `EfNotificationWriter` creates the existing Domain `Notification` entity and persists it through the same scoped `NamaaDbContext`. Business handlers contain no direct notification-repository writes. Domain remains free of MediatR, EF Core, and ASP.NET types.
- Events and recipients: `Project.TaskAssigned` targets the task's `AssignedToUserId` when an assigned task is created; `Project.MemberAdded` targets the added member's `UserId`; and `Invoice.Issued` targets the invoice's authoritative `CreatedByUserId`. The related task, member, or invoice ID is stored in the existing paired `EntityType`/`EntityId` fields. Project status/file events and the remaining finance workflows are deferred because the current model does not define one unambiguous recipient rule.
- Templates and content: the centralized catalog owns stable type names, titles, and messages. Messages contain only the task title, project ID, or invoice number as applicable; descriptions, notes, financial values, credentials, tokens, paths, and other free-form request bodies are not copied. The existing schema has no separate category or delivery-status column and was not changed.
- Self-service routes: the former `/api/users/{userId}/notifications` route was removed because its route user ID was not an ownership boundary. `GET /api/notifications`, `GET /api/notifications/{id}`, and `PATCH /api/notifications/{id}/read` now derive the owner exclusively from `ICurrentUser`. List supports the existing nullable `isRead` filter and orders by `CreatedAt DESC, Id DESC`. Repository lookups include recipient ID, so cross-user get and mark-read return the same safe `404`. Repeated read/unread transitions are idempotent. No HTTP notification-create route exists.
- Transactions and audit: notification event publication executes inside the existing `EfAuditTrail` transaction for the three audited source commands. A successful business mutation, in-app notification, and original business audit commit together; a failed Result, stale concurrency write, notification persistence exception, or later audit failure commits none of them. Notification persistence does not create a second audit record. The existing `Notification.ReadStateChanged` audit remains for owner-driven read-state commands.
- Logging and delivery: `EfNotificationWriter` logs persistence failures through `ILogger<T>` using recipient ID, stable type, related entity ID, and exception type, never message contents. `INotificationDeliveryProvider` defines the future external-delivery boundary, but no email, push, or SMS implementation is registered. In-app database persistence is the only active channel.
- Delivery guarantees: MediatR handlers run once per successful in-process command invocation. No idempotency column or Outbox was added. An Outbox, event identity/idempotency, retry policy, and provider delivery status become requirements if external or cross-process delivery is introduced.
- Tests: five SQL-backed acceptance cases verify task-assignee, added-member, and invoice-creator notifications; stable types and safe messages; joint business/notification/audit persistence; no notification on failed or stale mutations; self-only list/get/read-state access; ordering/filtering; anonymous `401`; arbitrary-create `405`; and removal of the unsafe user-ID route. Two focused Domain tests verify idempotent read/unread transitions.

## Security and infrastructure gaps

- User onboarding, password change/reset, lockout, MFA, and security-stamp invalidation remain undefined and are not exposed.
- Existing password hashes have no documented algorithm marker outside the hash itself. Login now accepts ASP.NET Core Identity password-hash formats; any legacy format requires an explicit compatibility/migration decision. Users whose persisted Email is null cannot use the email login endpoint.
- Maker-checker segregation-of-duties rules remain unresolved for finance and accounting workflows; the initial module-level role map intentionally does not invent creator-versus-poster restrictions.
- Project-level membership/ACL authorization is not yet defined; authenticated users currently share the same module policy boundary.
- Quotation rejection/cancellation, invoice issue/cancellation, and cancellation workflows do not have actor columns in the current Domain/schema, so no audit actor can be persisted for those transitions without a later schema/business decision.
- Workflow timestamps such as `PostedAt` remain caller-supplied to preserve current contracts; only actor identity was hardened in this phase.
- Project Files use protected local physical storage. Public deletion/archive semantics, malware scanning, content signature inspection, retention/reconciliation jobs, and project membership ACLs remain undefined.
- Project-level membership/ACL acceptance remains pending business authorization decisions. Administrative all-user notification access remains intentionally absent until a dedicated policy is defined.

## Remaining business decisions

- Define user onboarding, password reset/change, lockout/MFA, and security-stamp/token-family invalidation.
- Decide whether maker-checker segregation of duties, an audit-specific role, or administrative all-user notification access is required beyond the initial role map.
- Decide whether child-line replacement/removal is required before documents leave Draft; the current API supports safe addition and aggregate reads without exposing unsafe independent CRUD.
- Define recipient rules for additional notification events and adopt an Outbox with idempotent delivery if external providers or cross-process publication are introduced.
- Decide whether approved field-level before/after audit metadata or trusted client-IP capture is required; generic snapshots remain intentionally disabled.
- Select and configure a production object-storage provider, malware scanning, retention, and orphan reconciliation policy.
- Decide whether a future HTTP `ETag`/`If-Match` facade should supplement the current JSON Base64 `RowVersion` contract; the present contract is complete and consistent.

## Database and build safety

- No production migration was created or run.
- No configured application database or production data was used or modified. Tests created and dropped only random disposable databases.
- No connection string, credential, signing key, or fixed test password was committed.
- Final build command: `dotnet build Namaa.sln -m:1`.
- Final result: **0 warnings, 0 errors**.
- Final test command: `dotnet test Namaa.sln -m:1`; **79 passed, 0 failed, 0 skipped**.
