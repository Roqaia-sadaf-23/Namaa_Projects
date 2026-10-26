# 🌱 Namaa Projects

### Full-Stack Business Management Platform

**ASP.NET Core 8 • Angular 20 • SQL Server • Clean Architecture • CQRS**

Namaa Projects is a full-stack, modular business management platform designed to streamline project management, customer relationships, financial operations, human resources, procurement, and administrative workflows.

The platform combines an **ASP.NET Core REST API** with an **Angular web application**, following modern software engineering principles focused on maintainability, security, scalability, and testability.

**Project Status:** 🚧 Active Development

---

## 🚀 Technology Stack

| Category | Technologies |
|---|---|
| Backend | ASP.NET Core 8 Web API, C# |
| Frontend | Angular 20, TypeScript, HTML, CSS |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core |
| Architecture | Clean Architecture |
| Application Patterns | CQRS, MediatR, Repository Pattern, Result Pattern |
| Validation | FluentValidation |
| Authentication | JWT, Refresh Tokens |
| Authorization | Role-Based & Policy-Based Authorization |
| Logging | Serilog |
| API Documentation | Swagger / OpenAPI |
| Testing | .NET Test Project |
| Version Control | Git & GitHub |

## 🏗️ System Architecture

Namaa Projects follows **Clean Architecture**, separating business logic, application workflows, infrastructure, and HTTP presentation concerns.

### Backend Layers

**1. Domain Layer — `Namaa.Domain`**

Contains the core business model:

- Domain entities and enums
- Business rules and invariants
- Domain validation
- Business-specific behavior
- Framework-independent domain logic

**2. Application Layer — `Namaa.Application`**

Contains application use cases and abstractions:

- CQRS commands and queries
- MediatR handlers
- FluentValidation validators
- Data Transfer Objects (DTOs)
- Repository interfaces
- Application behaviors
- Result Pattern

**3. Infrastructure Layer — `Namaa.Infrastructure`**

Contains technical implementations:

- Entity Framework Core
- SQL Server persistence
- Entity configurations
- Repository implementations
- Database context
- Infrastructure dependency registration

**4. API Layer — `Namaa.Api`**

Provides HTTP access to application functionality:

- RESTful controllers
- JWT authentication
- Authorization policies
- Exception-handling middleware
- Swagger / OpenAPI
- Dependency injection composition
- Structured logging

### Dependency Direction

```text
                 Namaa.Api
                     |
          Dependency Composition
                     |
       +-------------+-------------+
       |                           |
Namaa.Application          Namaa.Infrastructure
       |                           |
       +----------+----------------+
                  |
             Namaa.Domain
```

Infrastructure implements abstractions defined by the inner layers. Domain business rules remain independent of infrastructure and HTTP concerns.

## 📁 Repository Structure

```text
Namaa_Projects/
│
├── Database/
│   └── NamaaDatabaseSchema.sql
│
├── docs/
│   └── BUSINESS_MODULES.md
│
├── frontend/
│   └── namaa-web/
│       └── Angular Application
│
├── src/
│   ├── Namaa.Domain/
│   ├── Namaa.Application/
│   ├── Namaa.Infrastructure/
│   └── Namaa.Api/
│
├── tests/
│   └── Namaa.Tests/
│
├── .gitignore
├── Directory.Build.props
├── Namaa.sln
└── README.md
```

The backend and frontend are maintained in the same repository to support coordinated full-stack development.

---

## 📦 Business Modules

Namaa Projects is organized into **five high-level business modules**.

### 1. 📋 Project Management

Supports project planning, organization, tracking, and collaboration.

Features and entities:

- Projects
- Project Stages
- Project Tasks
- Project Members
- Project Files
- Project Correspondences
- Project Status History

### 2. 🤝 CRM & Sales

Supports customer relationships, service management, and sales workflows.

Features and entities:

- Customers
- Customer Contacts
- Services
- Quotations
- Quotation Items
- Quotation Payment Terms

### 3. 💰 Finance & Accounting

Supports financial transactions, accounting records, and expense management.

Features and entities:

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

### 4. 👥 Human Resources (HR)

Supports internal user administration and access management.

Features and entities:

- Users
- Roles
- Authentication Support
- Refresh Token Management

### 5. 🏢 Procurement & Administration

Supports supplier relationships, purchasing, and administrative operations.

Features and entities:

- Suppliers
- Purchase Invoices
- Purchase Invoice Items
- Branches
- Notifications
- Audit Logs

The detailed business module ownership and feature mapping are documented in [BUSINESS_MODULES.md](docs/BUSINESS_MODULES.md).

**Note:** The module list represents the system's logical business scope. It does not imply that every feature has a completed frontend workflow.

---

## ⚙️ Backend Implementation

The backend is built around CQRS and Clean Architecture principles.

### CQRS

Write operations and read operations are modeled separately using commands and queries.

### MediatR

Application requests are dispatched to dedicated handlers, keeping controller logic minimal.

### Repository Pattern

Repository interfaces are defined in the Application layer, while implementations are maintained in Infrastructure.

### Result Pattern

Application operations return explicit success or failure results, helping controllers translate outcomes into consistent HTTP responses.

### FluentValidation

Request validation is separated from HTTP controllers and business persistence logic.

### Entity Framework Core

EF Core handles database persistence and maps domain entities to the existing SQL Server schema.

### Unit of Work Boundary

Repository implementations do not independently commit changes. Application handlers control persistence through the configured database abstraction.

---

## 🔐 Authentication & Authorization

The backend includes JWT-based authentication and an authorization foundation.

Implemented security components include:

- JWT access tokens using HMAC-SHA256
- Refresh token rotation
- SHA-256 hashed refresh token storage
- Role claims
- Current authenticated user abstraction
- Policy-based authorization
- Server-side actor identity assignment
- Authentication-aware API endpoints

### Authorization Policies

The system defines policies for:

- Project Management
- CRM & Sales
- Finance
- Journal Posting
- User Management
- Role Management
- Procurement
- Notifications
- Audit Logs

**Security Status:** Authentication and authorization infrastructure is implemented. Fine-grained permission enforcement and comprehensive security verification remain part of ongoing hardening work.

---

## 🗄️ Database Architecture

Namaa Projects uses Microsoft SQL Server with an existing relational database schema.

### Schema Overview

| Database Component | Count |
|---|---:|
| Tables | 32 |
| Columns | 357 |
| Primary Keys | 32 |
| Foreign Keys | 62 |
| Default Constraints | 75 |
| Check Constraints | 87 |
| Unique Constraints | 19 |
| Unique Indexes | 9 |

The schema uses relational constraints to protect data integrity and enforce key business relationships.

### Database Source

`Database/NamaaDatabaseSchema.sql`

EF Core mappings are maintained against the existing schema.

Database migrations, schema restructuring, and destructive database operations must be explicitly reviewed before execution.

---

## 🌐 Angular Frontend

**Location:** `frontend/namaa-web`

The frontend is built with Angular 20 and TypeScript.

It provides the user-facing foundation for interacting with the Namaa backend through REST APIs.

### Frontend Architecture

The frontend is being developed with an emphasis on:

- Modular feature organization
- Reusable Angular components
- Standalone components
- TypeScript
- Angular routing
- Services and dependency injection
- Responsive layouts
- Arabic and English localization
- RTL and LTR layout support
- Maintainable UI structure

### Frontend Development Status

The Angular application structure and multilingual, RTL-ready foundation are under active development.

Business screens, API integration, forms, dashboards, and complete module workflows are being developed incrementally.

### Planned Frontend Areas

- Authentication screens
- Main dashboard
- Project management workspace
- Customer management
- Quotations and sales
- Finance and accounting
- Human resources
- Procurement and administration
- User profile and settings

---

## 🔄 Example Request Flow

A typical API request follows this flow:

```text
Angular Frontend
       |
       v
ASP.NET Core Controller
       |
       v
MediatR Command / Query
       |
       v
FluentValidation
       |
       v
Application Handler
       |
       v
Repository Abstraction
       |
       v
EF Core Repository
       |
       v
SQL Server Database
       |
       v
Result / DTO
       |
       v
HTTP Response
       |
       v
Angular Frontend
```

This illustrates the logical workflow; individual requests may use different validation, authorization, or persistence paths.

---

## 🧪 Testing & Quality

The backend includes a dedicated test project:

`tests/Namaa.Tests`

Quality practices include:

- Automated tests
- Request validation
- Domain invariant enforcement
- Dependency separation
- Database mapping consistency
- Structured logging
- Error-handling consistency
- Security hardening

Run backend tests:

```bash
dotnet test Namaa.sln
```

Build the solution:

```bash
dotnet build Namaa.sln
```

Passing automated tests does not replace integration, security, and production-environment verification.

---

## 🚀 Getting Started

### Prerequisites

Install:

- .NET 8 SDK
- Node.js compatible with the frontend's Angular version
- Angular CLI (if using global CLI commands)
- Microsoft SQL Server
- Git
- Visual Studio or Visual Studio Code

### 1. Clone the Repository

```bash
git clone https://github.com/Roqaia-sadaf-23/Namaa_Projects.git
cd Namaa_Projects
```

### 2. Restore Backend Dependencies

```bash
dotnet restore Namaa.sln
```

### 3. Configure Database Connection

Set up a local SQL Server database according to the checked-in database schema.

The API uses .NET User Secrets for local credentials.

From the repository root:

```bash
cd src/Namaa.Api
```

Configure the database connection string:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<YOUR_CONNECTION_STRING>"
```

Configure the required JWT signing key and authentication settings according to the application's configuration.

Never commit credentials, JWT secrets, or connection strings containing passwords.

### 4. Run Backend API

From `src/Namaa.Api`:

```bash
dotnet run
```

The configured local API address is:

```text
http://localhost:5035
```

Swagger UI:

```text
http://localhost:5035/swagger
```

Actual addresses may vary according to launch profiles and environment settings.

### 5. Run Angular Frontend

Open another terminal from the repository root:

```bash
cd frontend/namaa-web
npm install
npm start
```

Use the development URL displayed by the Angular CLI.

Frontend API configuration must point to the appropriate running backend URL.

---

## 📊 Development Progress

| Area | Status |
|---|---|
| Clean Architecture Foundation | ✅ Implemented |
| Domain Entities | ✅ Implemented |
| EF Core Configurations | ✅ Implemented |
| Repository Infrastructure | ✅ Implemented |
| CQRS Application Layer | ✅ Implemented |
| FluentValidation | ✅ Implemented |
| JWT Authentication | ✅ Implemented |
| Refresh Token Rotation | ✅ Implemented |
| Authorization Policy Foundation | ✅ Implemented |
| REST API Controllers | ✅ Implemented |
| Result Pattern | ✅ Implemented |
| Structured Logging | ✅ Implemented |
| Angular Application Foundation | ✅ Implemented |
| Arabic / English and RTL Foundation | ✅ Implemented |
| Full Frontend Business Workflows | 🚧 In Progress |
| Fine-Grained Authorization | 🚧 In Progress |
| End-to-End Integration Tests | 🚧 In Progress |
| Production Deployment | 📋 Planned |

**Status note:** Implemented indicates an established technical component, not necessarily complete production certification or full business workflow coverage.

---

## 🎯 Roadmap

### Backend

- Complete remaining business workflow verification.
- Strengthen role and permission enforcement.
- Expand integration and security testing.
- Improve API contract documentation.
- Prepare production environment configuration.

### Frontend

- Complete shared layouts and navigation.
- Build reusable forms and UI components.
- Integrate authentication workflows.
- Connect Angular services to REST endpoints.
- Build business module screens and dashboards.
- Implement loading, validation, and error states.
- Expand responsive and multilingual UI testing.

### Deployment

- Establish production configuration.
- Configure secure hosting and HTTPS.
- Add automated CI/CD verification.
- Implement operational monitoring.
- Complete deployment and recovery documentation.

---

## 🎨 Design Principles

Namaa Projects prioritizes:

**Separation of Concerns** — Clear responsibility boundaries between layers.

**Maintainability** — Structured features and reusable abstractions.

**Security** — Authenticated access, policy enforcement, and protected credentials.

**Scalability** — Modular organization that supports incremental expansion.

**Testability** — Independently testable application behavior.

**Localization** — Support for Arabic, English, and RTL/LTR interfaces.

---

## 👩‍💻 Developer

**Roqaia Sadaf**

Full-Stack Developer

**Backend:** ASP.NET Core, C#, EF Core, SQL Server

**Frontend:** Angular, TypeScript, HTML, CSS

**Mobile:** Flutter, Dart

**Architecture:** Clean Architecture, CQRS, REST APIs

[GitHub](https://github.com/Roqaia-sadaf-23)

[Portfolio](https://roqaia-sadaf-23.github.io/)

---

### ⭐ Namaa Projects

*Building a modern, modular business management platform with ASP.NET Core and Angular.*

**Developed by Roqaia Sadaf.**
