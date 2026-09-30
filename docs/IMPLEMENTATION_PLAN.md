# Implementation plan and schema assessment

## Repository assessment — 2026-09-13

The supplied workspace was empty. There was no existing ASP.NET solution, C# code, SQL script, database project, connection string, or schema export to inspect. Consequently, there is no existing working code to preserve and no concrete database schema against which to validate mappings.

### Confirmed conflict / blocker

The request requires the already-designed SQL Server schema to be the source of truth, but its DDL is not present in this repository. The listed table names establish scope only; they do not establish fields, primary-key types, nullability, enum storage, precision/scale, indexes, foreign keys, checks, or `RowVersion` mappings. Creating EF entities now would redesign the schema by assumption, which conflicts with the brief.

**Required input before Phase 2:** the database creation script, SQL Server Database Project, or a read-only schema-only export for `NamaaProjects`. A redacted development connection string is useful for eventual integration verification but not required to begin mapping.

## Phased delivery

1. **Foundation — complete:** one `Namaa` solution with exactly four projects, correct dependency boundaries, baseline package references, Result Pattern, and global exception handling.
2. **Persistence model — blocked by schema:** map each table in per-entity EF configurations and verify against SQL Server; no migrations or destructive database actions.
3. **Application foundation:** result/error types, exception handler, CQRS registrations, FluentValidation, validation/logging/performance behaviors, and result-to-HTTP mapping.
4. **Identity:** password hashing, JWT/refresh-token workflow, roles, policies, audit hooks.
5. **Customer module:** customers and contacts, pagination/search, uniqueness and primary-contact rules.
6. **Quotation and project modules:** item/totals validation, lifecycle commands, approved-quotation conversion transaction, members/tasks/files.
7. **Billing modules:** invoices/items, payments and transactional balance/status logic, expenses.
8. **System modules:** notifications, audit-log query API, extensible notification and storage ports.
9. **API hardening:** filtering/sorting, OpenAPI/JWT documentation, production-safe logging and error responses.
10. **Tests:** focused unit tests for the stated rules and SQL Server-backed integration coverage for transactions and concurrency.

Each phase will build and run its relevant tests before the next begins.
