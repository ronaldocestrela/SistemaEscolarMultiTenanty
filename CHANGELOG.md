# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-11-01

### Added
- Initial project setup with Clean Architecture
- Multi-tenant support using Finbuckle.MultiTenant 9.4.0
- ASP.NET Core Identity integration for user management
- Entity Framework Core 9.0.9 with SQL Server support
- Database migrations and seeding capabilities
- Docker Compose configuration with SQL Server container
- OpenAPI/Swagger documentation
- School entity in Domain layer
- Application and Infrastructure layers following Clean Architecture principles
- RESTful Web API with .NET 9.0
- Comprehensive README documentation (English and Portuguese)
- GitIgnore configuration for .NET projects
- Multi-tenant database contexts (TenantDbContext and ApplicationDbContext)
- Role-based authorization system with configurable password policies
- Constants for claims, permissions, and roles
- Database initialization and seeding infrastructure
- Tenant isolation with header and claims strategies

### Technical Details
- **Framework**: .NET 9.0
- **Database**: SQL Server with Entity Framework Core
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, WebAPI layers)
- **Multi-tenancy**: Finbuckle.MultiTenant with EF Core store
- **Authentication**: ASP.NET Core Identity
- **Documentation**: OpenAPI/Swagger
- **Containerization**: Docker & Docker Compose

### Project Structure
```
ABCSchool/
├── src/
│   ├── core/
│   │   ├── Domain/          # Business entities and domain logic
│   │   └── Application/     # Use cases and application services
│   ├── Infrastructure/      # Data access and external services
│   └── WebAPI/             # API controllers and configuration
├── docker-compose.yaml     # Docker orchestration
├── README.md              # Comprehensive documentation
├── .gitignore            # Git ignore rules
└── ABCSchool.sln        # Visual Studio solution
```

### Database Schema
- **Tenant Management**: Multi-tenant configuration and isolation
- **Identity System**: Users, roles, and claims management
- **School Entities**: Basic school domain model

### Security Features
- Password requirements (8+ chars, mixed case, numbers, symbols)
- Unique email validation
- Role-based access control
- Tenant data isolation

[1.0.0]: https://github.com/ronaldocestrela/SistemaEscolarMultiTenanty/releases/tag/v1.0.0