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
- Customer
